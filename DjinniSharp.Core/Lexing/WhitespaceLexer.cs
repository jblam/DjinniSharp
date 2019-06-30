using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.Lexing
{
    class WhitespaceLexer : ILexPattern<char, DjinniLexTokenKind>
    {
        public DjinniLexTokenKind Kind => DjinniLexTokenKind.Whitespace;

        public bool TryConsume(char c) => char.IsWhiteSpace(c) && c != '\r' && c != '\n';
    }
}
