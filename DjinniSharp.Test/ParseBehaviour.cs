using DjinniSharp.Core.Lexing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Test
{
    using LexToken = LexToken<char, DjinniLexTokenKind>;
    using ParseToken = LexToken<LexToken<char, DjinniLexTokenKind>, Core.Parsing.ProductionKind>;
    using static DjinniLexTokenKind;
    using DjinniSharp.Core.Parsing;
    using System.Linq;
    using DjinniSharp.Core;

    [TestFixture]
    public class ParseBehaviour
    {
        static TestCaseData TestCase(IEnumerable<LexToken> input, IEnumerable<ParseToken> output) =>
            new TestCaseData(input)
            {
                ExpectedResult = output
            };
        static TestCaseData TestCase(IEnumerable<LexToken> input, IEnumerable<ProductionKind> output) =>
             new TestCaseData(input)
             {
                 ExpectedResult = output
             };
        static LexToken Lex(string input, DjinniLexTokenKind kind) => new LexToken(input.ToCharArray(), kind);
        public static IEnumerable<TestCaseData> GetTestCases() => new[]
        {
            TestCase(
                new[]{ Lex("@", Operator), Lex("a", Word), Lex(" ", Whitespace), Lex("b", Word), Lex("\n", Newline) },
                new[]
                {
                    new ParseToken(
                        new[]{ Lex("@", Operator), Lex("a", Word), Lex(" ", Whitespace), Lex("b", Word) }, ProductionKind.Directive),
                    new ParseToken(
                        new[]{ Lex("\n", Newline) }, ProductionKind.Null) }),
            TestCase(
                new[]{ Lex("#", Operator), Lex(" ", Whitespace), Lex("This", Word), Lex(" ", Whitespace), Lex("Comment", Whitespace), Lex("\n", Newline) },
                new[]
                {
                    new ParseToken(
                        new[]{ Lex("#", Operator), Lex(" ", Whitespace), Lex("This", Word), Lex(" ", Whitespace), Lex("Comment", Whitespace) }, ProductionKind.Comment),
                    new ParseToken(
                        new[]{ Lex("\n", Newline) }, ProductionKind.Null)
                }),
        };

        public static IEnumerable<TestCaseData> GetKindTestCases() => new[]
        {
            TestCase(
                new[]{ Lex("operation", Word), Lex("=", Operator), Lex("interface", Word), Lex("+", Operator), Lex("c", Word) },
                new[]{ ProductionKind.TypeDeclaration }),
        };

        [Test, TestCaseSource(nameof(GetTestCases))]
        public IEnumerable<object> ParsesTokens(object input) =>
            ParsesTokens((IEnumerable<LexToken>)input)
                .Cast<object>();
        internal IEnumerable<ParseToken> ParsesTokens(IEnumerable<LexToken> input)
        {
            var sut = new Parser();
            return sut.Consume(input);
        }

        [Test, TestCaseSource(nameof(GetKindTestCases))]
        public IEnumerable<object> ParsesTokensAsKind(object input) =>
            ParsesTokensAsKind((IEnumerable<LexToken>)input)
                .Cast<object>();
        internal IEnumerable<ProductionKind> ParsesTokensAsKind(IEnumerable<LexToken> input)
        {
            var sut = new Parser();
            return sut.Consume(input).Select(t => t.Kind);
        }

        [Test]
        public void ParsesSequenceOfThings()
        {
            var input = @"typename = interface +x +y {
    methodname(arg: type): type;
    const property: string = ""value"";
}";
            var lexer = new DjinniLexer();
            var parser = new Parser();
            var parsed = parser.Consume(lexer.Consume(input));
            CollectionAssert.AreEqual(
                new[]
                {
                    ProductionKind.TypeDeclaration,
                    ProductionKind.OpenBlock,
                    ProductionKind.MemberDeclaration,
                    ProductionKind.MemberDeclaration,
                    ProductionKind.CloseBlock
                },
                parsed.Select(t => t.Kind).Where(k => k != ProductionKind.Null));
        }

        [Test]
        public void ParsesInvalidMemberMissingSemi()
        {
            var input = @"@extern ""XYZ""
typename = struct
{
    invalid
}";

            var lexer = new DjinniLexer();
            var parser = new Parser();
            var parsed = parser.Consume(lexer.Consume(input));
            CollectionAssert.AreEqual(
                new[]
                {
                    ProductionKind.Directive,
                    ProductionKind.TypeDeclaration,
                    ProductionKind.OpenBlock,
                    ProductionKind.MemberDeclaration,
                    ProductionKind.CloseBlock
                },
                parsed.Select(t => t.Kind).Where(k => k != ProductionKind.Null));
        }
    }
}
