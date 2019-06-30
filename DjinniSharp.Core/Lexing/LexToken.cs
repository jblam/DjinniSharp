namespace DjinniSharp.Core.Lexing
{
    public struct LexToken<TKind>
    {
        public int Length { get; }
        public TKind Kind { get; }
        public static readonly LexToken<TKind> UnreadableToken = new LexToken<TKind>(1, default);
        public LexToken(int length, TKind kind)
        {
            Length = length;
            Kind = kind;
        }
        public override string ToString() => $"{Kind}({Length})";
    }
}
