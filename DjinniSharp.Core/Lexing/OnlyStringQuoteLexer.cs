using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.Lexing
{
    class OnlyStringQuoteLexer : ILexPattern<char, DjinniLexTokenKind>
    {
        public DjinniLexTokenKind Kind => DjinniLexTokenKind.Operator;

        bool hasMatched = false;

        public bool TryConsume(char c)
        {
            if (!hasMatched)
            {
                hasMatched = true;
                return c == '"';
            }
            return false;
        }
    }
}
