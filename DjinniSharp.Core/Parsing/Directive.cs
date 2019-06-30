using DjinniSharp.Core.Lexing;
using System.Collections.Generic;

namespace DjinniSharp.Core.Parsing
{
    using Token = LexToken<char, DjinniLexTokenKind>;

    class Directive : ILexPattern<Token, ProductionKind>
    {
        public ProductionKind Kind => ProductionKind.Directive;

        Token? directiveOperator;
        Token? directiveIdentifier;
        List<Token> contents;

        public bool TryConsume(Token token)
        {
            if (token.Kind == DjinniLexTokenKind.Newline)
                return false;
            if (directiveIdentifier.HasValue)
            {
                contents.Add(token);
                return true;
            }
            else if (directiveOperator.HasValue)
            {
                if (token.Kind == DjinniLexTokenKind.Word)
                {
                    directiveIdentifier = token;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (token.IsOperatorOf("@"))
            {
                directiveOperator = token;
                contents = new List<Token>();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
