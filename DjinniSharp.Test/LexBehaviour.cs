﻿using DjinniSharp.Core.Lexing;
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

        static LexToken<char, TKind> AsToken<TKind>(string content, TKind kind) =>
            new LexToken<char, TKind>(content.ToCharArray(), kind);


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
            protected override int GetLengthOfInput(char input) => 1;
            protected override IReadOnlyCollection<ILexPattern<char, ATokenKind>> GetAllPatterns() =>
                new[] { new SingleCharLexPattern('a', ATokenKind.A) };
        }
        class ABLexer : Lexer<char, ATokenKind>
        {
            protected override int GetLengthOfInput(char input) => 1;
            protected override IReadOnlyCollection<ILexPattern<char, ATokenKind>> GetAllPatterns() =>
                new[]
                {
                    new SingleCharLexPattern('a', ATokenKind.A),
                    new SingleCharLexPattern('b', ATokenKind.B)
                };
        }
        class MultiALexer : Lexer<char, ATokenKind>
        {
            protected override int GetLengthOfInput(char input) => 1;
            protected override IReadOnlyCollection<ILexPattern<char, ATokenKind>> GetAllPatterns() =>
                new[] { new MultiCharLexPattern('a', ATokenKind.A) };
        }

        [Test]
        public void CanParseA()
        {
            var lexer = new ALexer();
            var output = lexer.Consume("a");
            CollectionAssert.AreEqual(new[] { AsToken("a", ATokenKind.A) }, output);
        }

        [Test]
        public void CanParseAA()
        {
            var lexer = new ALexer();
            var output = lexer.Consume("aa");
            CollectionAssert.AreEqual(Enumerable.Repeat(AsToken("a", ATokenKind.A), 2), output);
        }

        [Test]
        public void CanParseABA()
        {
            var lexer = new ABLexer();
            var output = lexer.Consume("aba");
            CollectionAssert.AreEqual(
                new[] { AsToken("a", ATokenKind.A), AsToken("b", ATokenKind.B), AsToken("a", ATokenKind.A) },
                output);
        }

        [Test]
        public void CanParseBadChars()
        {
            var lexer = new ALexer();
            var output = lexer.Consume("axa");
            CollectionAssert.AreEqual(
                new[] { AsToken("a", ATokenKind.A), AsToken("x", ATokenKind.Unrecognised), AsToken("a", ATokenKind.A) },
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
                    AsToken("a", ATokenKind.A),
                    AsToken("xx", ATokenKind.Unrecognised),
                    AsToken("a", ATokenKind.A)
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
                    AsToken("x", ATokenKind.Unrecognised),
                    AsToken("a", ATokenKind.A)
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
                    AsToken("aa", ATokenKind.A),
                    AsToken("x", ATokenKind.Unrecognised),
                    AsToken("a", ATokenKind.A)
                },
                output);
        }
    }
}
