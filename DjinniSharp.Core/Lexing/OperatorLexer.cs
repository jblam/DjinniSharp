using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.Lexing
{
    class OperatorLexer : ILexPattern<char, DjinniLexTokenKind>
    {
        public DjinniLexTokenKind Kind => DjinniLexTokenKind.Operator;

        bool hasMatched = false;

        public bool TryConsume(char c)
        {
            if (!hasMatched)
            {
                hasMatched = true;
                switch (c)
                {
                    case '+':
                        // type exposure
                    case '=':
                        // assignment
                    case ':':
                        // type-defining
                    case ';':
                    case ',':
                        // sequence
                    case '"':
                        // string literal delimter
                    case '#':
                        // comment start
                    case '@':
                        // directive
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
    }
}
