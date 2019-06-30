using DjinniSharp.Core.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DjinniSharp.Core.Parsing
{
    using Token = LexToken<char, DjinniLexTokenKind>;

    class SingleOperatorDeclaration : ILexPattern<Token, ProductionKind>
    {
        public SingleOperatorDeclaration(DjinniLexTokenKind expectedKind, string expectedContent, ProductionKind kind)
        {
            Kind = kind;
            this.expectedKind = expectedKind;
            this.expectedContent = expectedContent;
        }

        public ProductionKind Kind { get; }
        readonly DjinniLexTokenKind expectedKind;
        readonly string expectedContent;
        bool isExpecting = true;

        public bool TryConsume(Token input)
        {
            if (isExpecting)
            {
                isExpecting = false;
                return input.Kind == expectedKind && input.Contents.SequenceEqual(expectedContent);
            }
            else
            {
                return false;
            }
        }
    }
}
