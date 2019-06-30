using DjinniSharp.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core
{
    internal class DjinniLexer : Lexer<char, DjinniLexTokenKind>
    {
        protected override IReadOnlyCollection<ILexPattern<char, DjinniLexTokenKind>> GetAllPatterns() =>
            new ILexPattern<char, DjinniLexTokenKind>[]
            {
                new WhitespaceLexer(),
                new NewlineLexer(),
                new WordLexer(),
                new OperatorLexer(),
                new BracketLexer(),
            };
    }
}
