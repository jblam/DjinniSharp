using System;
using System.Collections.Generic;
using System.Linq;

namespace DjinniSharp.Core.Lexing
{
    public struct LexToken<TContent, TKind> : IEquatable<LexToken<TContent, TKind>>
    {
        public IReadOnlyCollection<TContent> Contents => contents ?? none;
        public int Length => Contents.Count;
        public TKind Kind { get; }

        readonly IReadOnlyCollection<TContent> contents;
        static readonly IReadOnlyCollection<TContent> none = new TContent[0];

        public LexToken(IReadOnlyCollection<TContent> contents, TKind kind)
        {
            this.contents = contents ?? throw new ArgumentNullException(nameof(contents));
            Kind = kind;
        }
        public override string ToString() => $"{Kind}({Length})";

        public bool Equals(LexToken<TContent, TKind> other) =>
            Kind.Equals(other.Kind) && Contents.SequenceEqual(other.Contents);
    }
}
