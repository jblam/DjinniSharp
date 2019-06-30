using DjinniSharp.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core
{
    internal class DjinniLexer : Lexer<DjinniLexTokenKind>
    {
        protected override IReadOnlyCollection<ILexPattern<DjinniLexTokenKind>> GetAllPatterns() =>
            new ILexPattern<DjinniLexTokenKind>[]
            {
                new WhitespaceLexer(),
                new NewlineLexer(),
                new WordLexer(),
                new OperatorLexer(),
                new BracketLexer(),
            };
    }
}
