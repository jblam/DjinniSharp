using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DjinniSharp.Core
{
    class PositionalStreamReader
    {
        public PositionalStreamReader(Stream baseStream, Encoding encoding)
        {
            this.baseStream = baseStream;
            this.encoding = encoding;
            decoder = encoding.GetDecoder();
        }
        Stream baseStream;
        Encoding encoding;
        Decoder decoder;
        public char Next()
        {
            var existingPosition = baseStream.Position;
            var buffer = new byte[encoding.GetMaxByteCount(1)];
            var outBuffer = new char[1];
            baseStream.Read(buffer, 0, buffer.Length);
            decoder.Convert(buffer, 0, buffer.Length, outBuffer, 0, 1, true, out var bytesUsed, out var charsUsed, out var completed);
            baseStream.Position = Math.Min(baseStream.Length, existingPosition + bytesUsed);
            return outBuffer[0];
        }
        public int Position
        {
            get => (int)baseStream.Position;
            set => baseStream.Position = value;
        }
        public bool IsEndOfStream => baseStream.Position == baseStream.Length;
    }

    class StreamInput : PositionalInput<char>, IDisposable
    {
        public StreamInput(Stream s, Encoding encoding)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            if (!s.CanSeek)
                throw new ArgumentException("Stream must be seekable");
            if (encoding is null)
                throw new ArgumentNullException(nameof(encoding));
            reader = new PositionalStreamReader(s, encoding);
        }

        PositionalStreamReader reader;

        protected override int Position
        {
            get => (int)reader.Position;
            set => reader.Position = value;
        }

        protected override bool IsEndOfStream => reader.IsEndOfStream;

        protected override char Next() => (char)reader.Next();

        public void Dispose()
        {
            ((IDisposable)reader).Dispose();
        }
    }

    class StringInput : PositionalInput<char>
    {
        public StringInput(string source)
        {
            this.source = source;
        }

        readonly string source;

        protected override int Position { get; set; }

        protected override bool IsEndOfStream => Position >= source.Length;

        protected override char Next()
        {
            var output = source[Position];
            Position += 1;
            return output;
        }
    }


    abstract class PositionalInput<T>
    {
        public IEnumerable<T> GetEnumerable(int startPosition) => new Enumerable(this, startPosition, null);
        public IEnumerable<T> GetEnumerable(int startPosition, int count) => new Enumerable(this, startPosition, count);

        // simultaneously:
        // - provide an enumerator for Ts into tokens
        // - provide a positional input so we can inspect the underlying Ts that belong to a token
        //   - for bonus points, provide a second enumerator

        // Outer enumerator
        // while not at end of stream
        //   if I'm not the active enumerator, throw
        //   yield return next
        //
        // Inner enumerator
        // Set position
        // while not at end of stream
        //   if I'm not active, throw
        //   yield return next
        // ~ reset position


        /// <summary>
        /// Represents the position of the underlying stream in the abstract terms of that stream.
        /// No inference is made between the units of <see cref="Position"/> and the count of
        /// <typeparamref name="T"/> instances yielded.
        /// </summary>
        protected abstract int Position { get; set; }
        protected abstract bool IsEndOfStream { get; }
        protected abstract T Next();

        readonly Stack<NestedEnumerator> enumerators = new Stack<NestedEnumerator>();

        IEnumerator<T> Enumerate(int startPosition)
        {
            Position = startPosition;
            var output = new NestedEnumerator(this, Position, startPosition, null);
            enumerators.Push(output);
            return output;
        }
        IEnumerator<T> Enumerate(int startPosition, int count)
        {
            var output = new NestedEnumerator(this, Position, startPosition, count);
            Position = startPosition;
            enumerators.Push(output);
            return output;
        }

        void ThrowIfInactive(NestedEnumerator enumerator)
        {
            if (!ReferenceEquals(enumerators.Peek(), enumerator))
                throw new InvalidOperationException("Attempting to interact with an inactive enumerator");
        }
        void Close(NestedEnumerator enumerator)
        {
            ThrowIfInactive(enumerator);
            enumerators.Pop();
            Position = enumerator.ResetPosition;
        }

        (bool, T) MoveNext(NestedEnumerator enumerator)
        {
            ThrowIfInactive(enumerator);
            if (IsEndOfStream)
                return default;
            else
                return (true, Next());
        }

        struct Enumerable : IEnumerable<T>
        {
            public Enumerable(PositionalInput<T> source, int startPosition, int? count)
            {
                this.source = source;
                this.startPosition = startPosition;
                this.count = count;
            }
            PositionalInput<T> source;
            int startPosition;
            int? count;
            public IEnumerator<T> GetEnumerator() => count.HasValue
                ? source.Enumerate(startPosition, count.Value)
                : source.Enumerate(startPosition);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
        class NestedEnumerator : IEnumerator<T>
        {
            public NestedEnumerator(PositionalInput<T> parent, int resetPosition, int startPosition, int? count)
            {
                this.parent = parent;
                ResetPosition = resetPosition;
                StartPosition = startPosition;
                Count = count;
            }

            public int ResetPosition { get; }
            public int StartPosition { get; }
            public int? Count { get; }
            public bool IsDisposed { get; private set; }
            int yieldCount = 0;
            (bool didMove, T value) lastMove;
            readonly PositionalInput<T> parent;

            void ThrowIfDisposed()
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(NestedEnumerator));
            }

            public T Current
            {
                get
                {
                    ThrowIfDisposed();
                    return lastMove.didMove
                        ? lastMove.value
                        : throw new InvalidOperationException("Cannot provide a value");
                }
            }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                lastMove = default;
                IsDisposed = true;
                parent.Close(this);
            }

            public bool MoveNext()
            {
                ThrowIfDisposed();
                if (yieldCount >= Count)
                {
                    lastMove = default;
                    return false;
                }
                else
                {
                    lastMove = parent.MoveNext(this);
                    yieldCount += 1;
                    return lastMove.didMove;
                }
            }

            public void Reset() => throw new NotSupportedException();
        }
    }
}
