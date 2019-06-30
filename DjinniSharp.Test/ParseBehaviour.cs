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

    [TestFixture]
    public class ParseBehaviour
    {
        static TestCaseData TestCase(IEnumerable<LexToken> input, IEnumerable<ParseToken> output) =>
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
    }
}
