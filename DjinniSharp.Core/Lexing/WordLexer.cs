using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Core.Lexing
{
    class WordLexer : ILexPattern<char, DjinniLexTokenKind>
    {
        public DjinniLexTokenKind Kind => DjinniLexTokenKind.Word;

        bool isStarted, isAlreadyInvalid;

        public bool TryConsume(char c)
        {
            if (!isStarted)
            {
                isStarted = true;
                isAlreadyInvalid = !IsValidForFirstChar(c);
            }
            else
            {
                isAlreadyInvalid = !IsValidForNonfirstChar(c);
            }
            return !isAlreadyInvalid;
        }

        static bool IsValidForFirstChar(char c) =>
            char.IsLetter(c);
        static bool IsValidForNonfirstChar(char c) =>
            char.IsLetterOrDigit(c);
    }
}
