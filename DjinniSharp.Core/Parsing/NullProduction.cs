using DjinniSharp.Core.Lexing;

namespace DjinniSharp.Core.Parsing
{
    using Token = LexToken<char, DjinniLexTokenKind>;

    class NullProduction : ILexPattern<Token, ProductionKind>
    {
        public ProductionKind Kind => ProductionKind.Null;

        public bool TryConsume(Token input)
        {
            return input.Kind == DjinniLexTokenKind.Whitespace || input.Kind == DjinniLexTokenKind.Newline;
        }
    }
}
