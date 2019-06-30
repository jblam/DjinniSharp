using DjinniSharp.Core;
using DjinniSharp.Core.Lexing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DjinniSharp.Test
{
    using Token = LexToken<DjinniLexTokenKind>;
    using static DjinniLexTokenKind;

    [TestFixture]
    public class DjinniLexerBehaviour
    {
        static TestCaseData TestCase(string input, params LexToken<DjinniLexTokenKind>[] expectedOutput) =>
            new TestCaseData(input)
            {
                ExpectedResult = expectedOutput
            };

        public static IEnumerable<TestCaseData> TestCases => new[]
        {
            TestCase("asdf", new Token(4, Word)),
            TestCase("    asdf", new Token(4, Whitespace), new Token(4, Word)),
            TestCase("asdf asdf", new Token(4, Word), new Token(1, Whitespace), new Token(4, Word)),
            TestCase("asdf=asdf", new Token(4, Word), new Token(1, Operator), new Token(4, Word)),
            TestCase("asdf = asdf",
                new Token(4, Word),
                new Token(1, Whitespace),
                new Token(1, Operator),
                new Token(1, Whitespace),
                new Token(4, Word)),
            TestCase("const c = \"value\";",
                // `const c`
                new Token(5, Word),
                new Token(1, Whitespace),
                new Token(1, Word),
                // ` = `
                new Token(1, Whitespace),
                new Token(1, Operator),
                new Token(1, Whitespace),
                // `"value";
                new Token(1, Operator),
                new Token(5, Word),
                new Token(1, Operator),
                new Token(1, Operator)),
            TestCase("interface\r\n{\r\n}", 
                new Token(9, Word), 
                new Token(2, Newline), 
                new Token(1, OpenBracket),
                new Token(2, Newline), 
                new Token(1, CloseBracket)),
            TestCase("\r\n\r\n", new Token(2, Newline), new Token(2, Newline)),
            TestCase("\n\n", new Token(1, Newline), new Token(1, Newline)),
            TestCase("a(x:y):z;",
                new Token(1, Word),
                new Token(1, OpenBracket),
                new Token(1, Word),
                new Token(1, Operator),
                new Token(1, Word),
                new Token(1, CloseBracket),
                new Token(1, Operator),
                new Token(1, Word),
                new Token(1, Operator)),
        };

        static readonly DjinniLexer sut = new DjinniLexer();

        [Test, TestCaseSource(nameof(TestCases))]
        public IEnumerable<object> ProducesOutput(string input) => sut.Consume(input).Cast<object>();
    }
}
