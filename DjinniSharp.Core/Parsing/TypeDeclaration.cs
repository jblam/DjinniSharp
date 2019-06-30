using DjinniSharp.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.Parsing
{
    using Token = LexToken<char, DjinniLexTokenKind>;

    class TypeDeclaration : ILexPattern<Token, ProductionKind>
    {
        public ProductionKind Kind => ProductionKind.TypeDeclaration;

        bool isStarted;

        public bool TryConsume(Token input)
        {
            if (!isStarted)
            {
                isStarted = true;
                return input.Kind == DjinniLexTokenKind.Word;
            }
            if (input.Kind == DjinniLexTokenKind.OpenBracket || input.Kind == DjinniLexTokenKind.CloseBracket)
            {
                return false;
            }
            return true;
        }
    }
}
