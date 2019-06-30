using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.Lexing
{
    class NewlineLexer : ILexPattern<char, DjinniLexTokenKind>
    {
        public DjinniLexTokenKind Kind => DjinniLexTokenKind.Newline;

        bool hasSeenCr, hasSeenLf;

        public bool TryConsume(char c)
        {
            if (hasSeenLf)
                return false;
            switch (c)
            {
                case '\r':
                    var output = !hasSeenCr;
                    hasSeenCr = true;
                    return output;
                case '\n':
                    hasSeenLf = true;
                    return true;
                default:
                    return false;
            }
        }
    }
}
