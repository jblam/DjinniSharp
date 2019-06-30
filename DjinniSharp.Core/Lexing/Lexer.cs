﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DjinniSharp.Core.Lexing
{
    public abstract class Lexer<TInput, TTokenKind>
    {
        protected abstract IReadOnlyCollection<ILexPattern<TInput, TTokenKind>> GetAllPatterns();
        protected virtual TTokenKind ErrorKind => default;
        protected virtual IEqualityComparer<TTokenKind> EqualityComparer { get; } = EqualityComparer<TTokenKind>.Default;

        static IReadOnlyCollection<ILexPattern<TInput, TTokenKind>> TryConsumeAll(IReadOnlyCollection<ILexPattern<TInput, TTokenKind>> availablePatterns, TInput c)
        {
            var livePatterns = new List<ILexPattern<TInput, TTokenKind>>();
            foreach (var pattern in availablePatterns)
            {
                if (pattern.TryConsume(c))
                {
                    livePatterns.Add(pattern);
                }
            }
            return livePatterns;
        }

        public IEnumerable<LexToken<TTokenKind>> Consume(IEnumerable<TInput> input)
        {
            int unreadableLength = 0;
            foreach (var token in ConsumeImpl())
            {
                if (EqualityComparer.Equals(token.Kind, ErrorKind))
                {
                    unreadableLength += token.Length;
                }
                else
                {
                    if (unreadableLength != 0)
                        yield return new LexToken<TTokenKind>(unreadableLength, ErrorKind);
                    unreadableLength = 0;
                    yield return token;
                }
            }
            if (unreadableLength != 0)
                yield return new LexToken<TTokenKind>(unreadableLength, ErrorKind);


            IEnumerable<LexToken<TTokenKind>> ConsumeImpl()
            {
                int tokenStartPosition = 0;
                int currentLexPosition = 0;
                IReadOnlyCollection<ILexPattern<TInput, TTokenKind>> availablePatterns = GetAllPatterns();
                foreach (var c in input)
                {
                    int existingTokenConsumedSize = currentLexPosition - tokenStartPosition;

                    var livePatterns = TryConsumeAll(availablePatterns, c);

                    if (!livePatterns.Any())
                    {
                        if (existingTokenConsumedSize > 0)
                            yield return ConstructToken(existingTokenConsumedSize, availablePatterns);

                        // reset, and attempt to consume the token again
                        availablePatterns = TryConsumeAll(GetAllPatterns(), c);
                        if (availablePatterns.Any())
                        {
                            // start the token for `c`
                            tokenStartPosition = currentLexPosition;
                        }
                        else
                        {
                            // `c` is not readable; yield a one-char unreadable token, and
                            // start the next token at the next char
                            yield return new LexToken<TTokenKind>(1, ErrorKind);
                            tokenStartPosition = currentLexPosition + 1;
                        }
                    }
                    else
                    {
                        // at least one pattern was able to consume the token
                        availablePatterns = livePatterns;
                    }

                    currentLexPosition += 1;
                }

                if (tokenStartPosition != currentLexPosition)
                {
                    // yield final token
                    yield return ConstructToken(currentLexPosition - tokenStartPosition, availablePatterns);
                }
            }
        }

        LexToken<TTokenKind> ConstructToken(int length, IEnumerable<ILexPattern<TInput, TTokenKind>> candidatePatterns)
        {
            if (length == 0)
                throw new ArgumentException("Attempted to construct a zero-length token");
            if (!candidatePatterns.TryGetSingle(out var pattern))
                return new LexToken<TTokenKind>(length, ErrorKind);
            return new LexToken<TTokenKind>(length, pattern.Kind);
        }
    }

    static class EnumerableExtensions
    {
        public static bool TryGetExactly<T>(this IEnumerable<T> input, int count, out IReadOnlyList<T> values)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("Count must be greater than zero");
            if (input is null)
                throw new ArgumentNullException(nameof(input));
            values = null;
            var output = new List<T>(count);
            using (var e = input.GetEnumerator())
                while (e.MoveNext())
                {
                    if (output.Count >= count)
                        return false;
                    output.Add(e.Current);
                }
            if (output.Count != count)
                return false;
            values = output;
            return true;
        }
        public static bool TryGetSingle<T>(this IEnumerable<T> input, out T t)
        {
            var result = input.TryGetExactly(1, out var value);
            t = result ? value[0] : default;
            return result;
        }
    }
}
