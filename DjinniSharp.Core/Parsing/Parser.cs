using DjinniSharp.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.Parsing
{
    using Token = LexToken<DjinniLexTokenKind>;

    class Parser : Lexer<Token, ProductionKind>
    {
        protected override IReadOnlyCollection<ILexPattern<Token, ProductionKind>> GetAllPatterns() =>
            new ILexPattern<Token, ProductionKind>[]
            {
                new NullProduction(),
                new Directive(),
                // new Comment(),
            };
    }

    enum ProductionKind
    {
        Unrecognised,
        Null,
        Directive,
        Comment,
    }
    class NullProduction : ILexPattern<Token, ProductionKind>
    {
        public ProductionKind Kind => ProductionKind.Null;

        public bool TryConsume(Token input)
        {
            return input.Kind == DjinniLexTokenKind.Whitespace || input.Kind == DjinniLexTokenKind.Newline;
        }
    }

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
            else if (token.Kind == DjinniLexTokenKind.Operator)
            {
                // TODO: ensure it's the *right* operator
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
            else if (input.Kind == DjinniLexTokenKind.Operator)
            {
                // TODO: ensure it's the *right* operator
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

    struct ParseToken
    {
        int Offset { get; }
        LexToken<DjinniLexTokenKind> Token { get; }
    }

    class Production
    {
        int Position { get; }
        class Directive : Production
        {
            ParseToken DirectiveOperator { get; }
            ParseToken DirectiveIdentifier { get; }
            ParseToken ArgumentToken { get; }
        }
        class Comment : Production
        {
            ParseToken CommentOperator { get; }
            IReadOnlyCollection<ParseToken> Content { get; }
        }
        class TypeDeclaration : Production
        {
            ParseToken NameIdentifier { get; }
            ParseToken AssignmentOperator { get; }
            ParseToken Keyword { get; }
            IReadOnlyCollection<(ParseToken, ParseToken)> ExposureOperators { get; }
        }
        class MemberDeclaration : Production
        {
            IReadOnlyCollection<ParseToken> ModifierKeywords { get; }
            ParseToken MemberIdentifier { get; }
        }
        class MethodArguments : Production
        {
            ParseToken OpenBracket { get; }
            IReadOnlyCollection<(ParseToken Identifier, ParseToken Colon, ParseToken Type)> Arguments { get; }
            ParseToken CloseBracket { get; }
        }
    }
}
