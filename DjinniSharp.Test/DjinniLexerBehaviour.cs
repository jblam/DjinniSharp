using DjinniSharp.Core;
using DjinniSharp.Core.Lexing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DjinniSharp.Test
{
    using Token = LexToken<char, DjinniLexTokenKind>;
    using static DjinniLexTokenKind;

    [TestFixture]
    public class DjinniLexerBehaviour
    {
        static TestCaseData TestCase(string input, params Token[] expectedOutput) =>
            new TestCaseData(input)
            {
                ExpectedResult = expectedOutput
            };

        static Token AsToken(string content, DjinniLexTokenKind kind) =>
            new Token(content.ToCharArray(), kind);

        static readonly Token
            Space = AsToken(" ", Whitespace),
            CrLf = AsToken("\r\n", Newline),
            Equal = AsToken("=", Operator),
            Quote = AsToken("\"", Operator),
            Asdf = AsToken("asdf", Word);

        public static IEnumerable<TestCaseData> TestCases => new[]
        {
            TestCase("asdf", Asdf),
            TestCase("    asdf", AsToken("    ", Whitespace), Asdf),
            TestCase("asdf asdf", Asdf, Space, Asdf),
            TestCase("asdf=asdf", Asdf, Equal, Asdf),
            TestCase("asdf = asdf", Asdf, Space, Equal, Space, Asdf),
            TestCase("const c = \"value\";",
                AsToken("const", Word), Space, AsToken("c", Word),
                Space, Equal, Space,
                Quote, AsToken("value", Word), Quote, AsToken(";", Operator)),
            TestCase("interface\r\n{\r\n}", 
                AsToken("interface", Word), CrLf, AsToken("{", OpenBracket), CrLf, AsToken("}", CloseBracket)),
            TestCase("\r\n\r\n", CrLf, CrLf),
            TestCase("\n\n", AsToken("\n", Newline), AsToken("\n", Newline)),
            TestCase("a(x:y):z;",
                AsToken("a", Word),
                AsToken("(", OpenBracket),
                AsToken("x", Word),
                AsToken(":", Operator),
                AsToken("y", Word),
                AsToken(")", CloseBracket),
                AsToken(":", Operator),
                AsToken("z", Word),
                AsToken(";", Operator)),
            TestCase("\"}\"",
                Quote, AsToken("}", Word), Quote),
            TestCase("\"}\r\n",
                Quote, AsToken("}", Word), CrLf),
        };

        static readonly DjinniLexer sut = new DjinniLexer();

        [Test, TestCaseSource(nameof(TestCases))]
        public IEnumerable<object> ProducesOutput(string input) => sut.Consume(input).Cast<object>();
    }
}
