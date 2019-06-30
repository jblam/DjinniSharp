namespace DjinniSharp.Core.Lexing
{
    public interface ILexPattern<TInput, TKind>
    {
        bool TryConsume(TInput input);
        TKind Kind { get; }
    }
}
