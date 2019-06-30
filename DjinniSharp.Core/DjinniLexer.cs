using DjinniSharp.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core
{
    internal class DjinniLexer : Lexer<char, DjinniLexTokenKind>
    {
        bool isInQuotedString = false;

        protected override void OnTokenProduced(LexToken<char, DjinniLexTokenKind> token)
        {
            if (token.IsOperatorOf("\""))
                isInQuotedString = !isInQuotedString;
            if (token.Kind == DjinniLexTokenKind.Newline)
                isInQuotedString = false;
        }

        protected override IReadOnlyCollection<ILexPattern<char, DjinniLexTokenKind>> GetAllPatterns()
        {
            if (isInQuotedString)
                return new ILexPattern<char, DjinniLexTokenKind>[]
                {
                    new StringLiteralContentLexer(),
                    new OnlyStringQuoteLexer(),
                    new NewlineLexer(),
                };
            else
                return new ILexPattern<char, DjinniLexTokenKind>[]
                {
                    new WhitespaceLexer(),
                    new NewlineLexer(),
                    new WordLexer(),
                    new OperatorLexer(),
                    new BracketLexer(),
                };
        }
    }
}
