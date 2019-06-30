using System;
using System.Collections.Generic;
using System.Linq;

namespace DjinniSharp.Core.Lexing
{
    public struct LexToken<TContent, TKind> : IEquatable<LexToken<TContent, TKind>>
    {
        public IReadOnlyCollection<TContent> Contents { get; }
        public int Length => Contents.Count;
        public TKind Kind { get; }
        public LexToken(IReadOnlyCollection<TContent> contents, TKind kind)
        {
            Contents = contents;
            Kind = kind;
        }
        public override string ToString() => $"{Kind}({Length})";

        public bool Equals(LexToken<TContent, TKind> other) =>
            Kind.Equals(other.Kind) && Contents.SequenceEqual(other.Contents);
    }
}
