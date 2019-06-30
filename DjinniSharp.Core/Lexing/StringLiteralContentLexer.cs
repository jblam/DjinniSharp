using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.Lexing
{
    class StringLiteralContentLexer : ILexPattern<char, DjinniLexTokenKind>
    {
        public DjinniLexTokenKind Kind => DjinniLexTokenKind.Word;

        public bool TryConsume(char input)
        {
            return input != '"' && input != '\r' && input != '\n';
        }
    }
}
