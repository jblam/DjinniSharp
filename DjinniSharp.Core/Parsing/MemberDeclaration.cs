using DjinniSharp.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DjinniSharp.Core.Parsing
{
    using Token = LexToken<char, DjinniLexTokenKind>;

    class MemberDeclaration : ILexPattern<Token, ProductionKind>
    {
        public ProductionKind Kind => ProductionKind.MemberDeclaration;

        bool isStarted;
        bool hasSeenSemi;

        public bool TryConsume(Token input)
        {
            if (!isStarted && input.Kind != DjinniLexTokenKind.Word)
                return false;
            if (hasSeenSemi)
                return false;
            if (input.Kind == DjinniLexTokenKind.CloseBracket && input.Contents.SequenceEqual("}"))
                return false;
            if (input.IsOperatorOf(";"))
            {
                hasSeenSemi = true;
                return true;
            }
            isStarted = true;
            return true;
        }
    }
}
