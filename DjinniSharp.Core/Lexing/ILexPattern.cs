namespace DjinniSharp.Core.Lexing
{
    public interface ILexPattern<TKind>
    {
        bool TryConsume(char c);
        TKind Kind { get; }
    }
}
