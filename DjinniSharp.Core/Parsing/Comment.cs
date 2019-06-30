using DjinniSharp.Core.Lexing;
using System.Collections.Generic;
using System.Linq;

namespace DjinniSharp.Core.Parsing
{
    using Token = LexToken<char, DjinniLexTokenKind>;

    class Comment : ILexPattern<Token, ProductionKind>
    {
        public ProductionKind Kind => ProductionKind.Comment;

        Token? commentOperator;
        List<Token> contents;

        public bool TryConsume(Token input)
        {
            if (input.Kind == DjinniLexTokenKind.Newline)
                return false;
            else if (commentOperator.HasValue)
            {
                contents.Add(input);
                return true;
            }
            else if (input.IsOperatorOf("#"))
            {
                commentOperator = input;
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
