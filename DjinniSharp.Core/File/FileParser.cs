using DjinniSharp.Core.Lexing;
using DjinniSharp.Core.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.File
{
    using ProductionToken = LexToken<LexToken<char, DjinniLexTokenKind>, ProductionKind>;

    class FileParser : Lexer<ProductionToken, FileSection>
    {
        protected override int GetLengthOfInput(ProductionToken input) => input.Length;

        protected override void OnTokenProduced(LexToken<ProductionToken, FileSection> token)
        {
            if (token.Kind == FileSection.Type)
                hasSeenType = true;
        }

        protected override IReadOnlyCollection<ILexPattern<ProductionToken, FileSection>> GetAllPatterns()
        {
            if (hasSeenType)
                return new ILexPattern<ProductionToken, FileSection>[]
                {
                    new EmptySection(),
                    new TypeSection(),
                };
            else
                return new ILexPattern<ProductionToken, FileSection>[]
                {
                    new DirectivesSection(),
                    new EmptySection(),
                    new TypeSection(),
                };
        }

        bool hasSeenType;
    }

    enum FileSection
    {
        Invalid,
        Empty,
        Directives,
        Type,
    }
}
