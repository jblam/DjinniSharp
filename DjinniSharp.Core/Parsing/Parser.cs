using DjinniSharp.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DjinniSharp.Core.Parsing
{
    using Token = LexToken<char, DjinniLexTokenKind>;

    class Parser : Lexer<Token, ProductionKind>
    {
        protected override void OnTokenProduced(LexToken<Token, ProductionKind> token)
        {
            if (token.Kind == ProductionKind.OpenBlock)
                isInBlock = true;
            else if (token.Kind == ProductionKind.CloseBlock)
                isInBlock = false;
        }

        protected override int GetLengthOfInput(Token input) => input.Length;

        protected override IReadOnlyCollection<ILexPattern<Token, ProductionKind>> GetAllPatterns()
        {
            if (isInBlock)
                return new ILexPattern<Token, ProductionKind>[]
                {
                    new NullProduction(),
                    new Directive(),
                    new Comment(),
                    new MemberDeclaration(),
                    new SingleOperatorDeclaration(DjinniLexTokenKind.OpenBracket, "{", ProductionKind.OpenBlock),
                    new SingleOperatorDeclaration(DjinniLexTokenKind.CloseBracket, "}", ProductionKind.CloseBlock)
                };
            else
                return new ILexPattern<Token, ProductionKind>[]
                {
                    new NullProduction(),
                    new Directive(),
                    new Comment(),
                    new TypeDeclaration(),
                    new SingleOperatorDeclaration(DjinniLexTokenKind.OpenBracket, "{", ProductionKind.OpenBlock),
                    new SingleOperatorDeclaration(DjinniLexTokenKind.CloseBracket, "}", ProductionKind.CloseBlock)
                };
        }
        bool isInBlock = false;
    }

    enum ProductionKind
    {
        Unrecognised,
        Null,
        Directive,
        Comment,
        TypeDeclaration,
        MemberDeclaration,
        OpenBlock,
        CloseBlock,
    }
}
