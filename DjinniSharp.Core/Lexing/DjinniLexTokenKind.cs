using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.Lexing
{
    enum DjinniLexTokenKind
    {
        Unrecognised,
        Whitespace,
        Word,
        Operator,
        Newline,
        OpenBracket,
        CloseBracket
    }
}
