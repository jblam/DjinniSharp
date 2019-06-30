using DjinniSharp.Core.Lexing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DjinniSharp.Test
{
    [TestFixture]
    public class LexBehaviour
    {
        enum ATokenKind
        {
            Unrecognised,
            A,
            B
        }
        class SingleCharLexPattern : ILexPattern<char, ATokenKind>
        {
            public SingleCharLexPattern(char expected, ATokenKind kind)
            {
                this.expected = expected;
                Kind = kind;
            }

            public ATokenKind Kind { get; }

            readonly char expected;
            char? firstChar = null;

            public bool TryConsume(char c)
            {
                if (firstChar.HasValue)
                    return false;
                firstChar = c;
                return firstChar == expected;
            }
        }
        class MultiCharLexPattern : ILexPattern<char, ATokenKind>
        {
            public MultiCharLexPattern(char expected, ATokenKind kind)
            {
                Kind = kind;
                this.expected = expected;
            }
            public ATokenKind Kind { get; }
            readonly char expected;

            public bool TryConsume(char c) => c == expected;
        }

        class ALexer : Lexer<char, ATokenKind>
        {
            protected override IReadOnlyCollection<ILexPattern<char, ATokenKind>> GetAllPatterns() =>
                new[] { new SingleCharLexPattern('a', ATokenKind.A) };
        }
        class ABLexer : Lexer<char, ATokenKind>
        {
            protected override IReadOnlyCollection<ILexPattern<char, ATokenKind>> GetAllPatterns() =>
                new[]
                {
                    new SingleCharLexPattern('a', ATokenKind.A),
                    new SingleCharLexPattern('b', ATokenKind.B)
                };
        }
        class MultiALexer : Lexer<char, ATokenKind>
        {
            protected override IReadOnlyCollection<ILexPattern<char, ATokenKind>> GetAllPatterns() =>
                new[] { new MultiCharLexPattern('a', ATokenKind.A) };
        }

        [Test]
        public void CanParseA()
        {
            var lexer = new ALexer();
            var output = lexer.Consume("a");
            CollectionAssert.AreEqual(new[] { new LexToken<ATokenKind>(1, ATokenKind.A) }, output);
        }

        [Test]
        public void CanParseAA()
        {
            var lexer = new ALexer();
            var output = lexer.Consume("aa");
            CollectionAssert.AreEqual(Enumerable.Repeat(new LexToken<ATokenKind>(1, ATokenKind.A), 2), output);
        }

        [Test]
        public void CanParseABA()
        {
            var lexer = new ABLexer();
            var output = lexer.Consume("aba");
            CollectionAssert.AreEqual(
                new[] { ATokenKind.A, ATokenKind.B, ATokenKind.A }.Select(t => new LexToken<ATokenKind>(1, t)),
                output);
        }

        [Test]
        public void CanParseBadChars()
        {
            var lexer = new ALexer();
            var output = lexer.Consume("axa");
            CollectionAssert.AreEqual(
                new[] { ATokenKind.A, ATokenKind.Unrecognised, ATokenKind.A }.Select(t => new LexToken<ATokenKind>(1, t)),
                output);
        }

        [Test]
        public void AccumulatesBadChars()
        {
            var lexer = new ALexer();
            var output = lexer.Consume("axxa");
            CollectionAssert.AreEqual(
                new[]
                {
                    new LexToken<ATokenKind>(1, ATokenKind.A),
                    new LexToken<ATokenKind>(2, ATokenKind.Unrecognised),
                    new LexToken<ATokenKind>(1, ATokenKind.A)
                },
                output);
        }

        [Test]
        public void HandlesLeadingBadChar()
        {
            var lexer = new ALexer();
            var output = lexer.Consume("xa");
            CollectionAssert.AreEqual(
                new[]
                {
                    new LexToken<ATokenKind>(1, ATokenKind.Unrecognised),
                    new LexToken<ATokenKind>(1, ATokenKind.A)
                },
                output);
        }

        [Test]
        public void HandlesMulticharPattern()
        {
            var lexer = new MultiALexer();
            var output = lexer.Consume("aaxa");
            CollectionAssert.AreEqual(
                new[]
                {
                    new LexToken<ATokenKind>(2, ATokenKind.A),
                    new LexToken<ATokenKind>(1, ATokenKind.Unrecognised),
                    new LexToken<ATokenKind>(1, ATokenKind.A)
                },
                output);
        }
    }
}
