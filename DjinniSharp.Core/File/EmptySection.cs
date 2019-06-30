using DjinniSharp.Core.Lexing;
using DjinniSharp.Core.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.File
{
    using ProductionToken = LexToken<LexToken<char, DjinniLexTokenKind>, ProductionKind>;

    class EmptySection : ILexPattern<ProductionToken, FileSection>
    {
        public FileSection Kind => FileSection.Empty;

        public bool TryConsume(ProductionToken input)
        {
            return input.Kind == ProductionKind.Null
                || input.Kind == ProductionKind.Comment;
        }
    }
}
