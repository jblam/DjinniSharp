using DjinniSharp.Core.Lexing;
using DjinniSharp.Core.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.File
{
    using ProductionToken = LexToken<LexToken<char, DjinniLexTokenKind>, ProductionKind>;

    class TypeSection : ILexPattern<ProductionToken, FileSection>
    {
        public FileSection Kind => FileSection.Type;

        public bool TryConsume(ProductionToken input)
        {
            if (hasClosed)
                return false;
            switch (input.Kind)
            {
                case ProductionKind.Unrecognised:
                    return hasSeenNotEmpty;
                case ProductionKind.Null:
                    return hasSeenNotEmpty;
                case ProductionKind.Directive:
                    return false;
                case ProductionKind.CloseBlock:
                    hasClosed = true;
                    return true;
                case ProductionKind.TypeDeclaration:
                    if (hasSeenDeclaration)
                        return false;
                    else
                    {
                        hasSeenDeclaration = true;
                        hasSeenNotEmpty = true;
                        return true;
                    }
                default:
                    hasSeenNotEmpty = true;
                    return true;
            }
        }

        bool hasSeenNotEmpty,
            hasSeenDeclaration,
            hasOpened,
            hasClosed;
    }
}
