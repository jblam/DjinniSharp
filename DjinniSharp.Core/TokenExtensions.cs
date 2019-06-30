using DjinniSharp.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DjinniSharp.Core
{
    static class TokenExtensions
    {
        public static bool IsOperatorOf(this LexToken<char, DjinniLexTokenKind> token, string op) =>
            token.Kind == DjinniLexTokenKind.Operator &&
            token.Contents.SequenceEqual(op);
        public static bool IsWhitespaceOrNewline(this LexToken<char, DjinniLexTokenKind> token) =>
            token.Kind == DjinniLexTokenKind.Whitespace || token.Kind == DjinniLexTokenKind.Newline;
    }
}
