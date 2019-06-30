namespace DjinniSharp.Core
{
    public interface ILexPattern<TKind>
    {
        bool TryConsume(char c);
        TKind Kind { get; }
    }
}
