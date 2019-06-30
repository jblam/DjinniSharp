using DjinniSharp.Core.Lexing;

namespace DjinniSharp.Core
{
    internal class BracketLexer : ILexPattern<DjinniLexTokenKind>
    {
        public DjinniLexTokenKind Kind { get; private set; } = default;

        public bool TryConsume(char c)
        {
            if (Kind == default)
            {
                switch (c)
                {
                    case '{':
                    case '(':
                        Kind = DjinniLexTokenKind.OpenBracket;
                        return true;
                    case '}':
                    case ')':
                        Kind = DjinniLexTokenKind.CloseBracket;
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }
    }
}