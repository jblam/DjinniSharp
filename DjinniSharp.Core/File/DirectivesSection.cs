using DjinniSharp.Core.Lexing;
using DjinniSharp.Core.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.File
{
    using ProductionToken = LexToken<LexToken<char, DjinniLexTokenKind>, ProductionKind>;

    class DirectivesSection : ILexPattern<ProductionToken, FileSection>
    {
        public FileSection Kind => FileSection.Directives;

        public bool TryConsume(ProductionToken input)
        {
            if (!hasStarted)
            {
                hasStarted = true;
                return input.Kind == ProductionKind.Directive
                    || input.Kind == ProductionKind.Comment;
            }
            else
            {
                return input.Kind == ProductionKind.Null
                    || input.Kind == ProductionKind.Directive
                    || input.Kind == ProductionKind.Comment;
            }
        }

        bool hasStarted;
    }
}
