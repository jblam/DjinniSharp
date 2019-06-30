using DjinniSharp.Core.Lexing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Test
{
    using LexToken = LexToken<DjinniLexTokenKind>;
    using ParseToken = LexToken<Core.Parsing.ProductionKind>;
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
        static LexToken Lex(DjinniLexTokenKind kind) => new LexToken(0, kind);
        public static IEnumerable<TestCaseData> GetTestCases() => new[]
        {
            TestCase(
                new[]{ Lex(Operator), Lex(Word), Lex(Whitespace), Lex(Word), Lex(Newline) },
                new[]{ new ParseToken(4, ProductionKind.Directive), new ParseToken(1, ProductionKind.Null) }),
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
