using DjinniSharp.Core.Lexing;
using DjinniSharp.Core.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DjinniSharp.Core.Interpreting
{
    using CharToken = LexToken<char, DjinniLexTokenKind>;

    class TypeDeclaration
    {
        public string Identifier { get; }
        public TypeKind Kind { get; }
        public IReadOnlyCollection<string> Exposures { get; }

        public static TypeDeclaration Interpret(LexToken<CharToken, ProductionKind> production)
        {
            if (production.Kind != ProductionKind.TypeDeclaration)
                throw new ArgumentException("Unepxected production kind");
            var nonEmptyTokens = production.Contents.Where(c => c.Kind != DjinniLexTokenKind.Whitespace && c.Kind != DjinniLexTokenKind.Newline);
            var stepper = new Stepper<CharToken>(nonEmptyTokens);
            ExpectIdentifier(stepper.NextOrDefault())
                .Then(i => ExpectEquals(i, stepper.NextOrDefault()))
        }

        public static Expectation<CharToken> ExpectIdentifier(CharToken next)
        {
            if (next.Kind != DjinniLexTokenKind.Word)
                return Expectation.Invalid(next, "Identifier must be a word");
            if (!char.IsLetter(next.Contents.FirstOrDefault()))
                return Expectation.Invalid(next, "Identifier must begin with a letter");
            return Expectation.Valid(next);
        }
        public static Expectation<CharToken> ExpectEquals(CharToken identifier, CharToken next)
        {
            if (next.IsOperatorOf("="))
                return Expectation.Valid(identifier);
            else
                return Expectation.Invalid(next, "Expected equals operator");
        }
        public static Expectation<(CharToken, TypeKind)> ExpectKeyword(CharToken identifier, CharToken keyword)
        {
            if (Enum.TryParse<TypeKind>(new string(keyword.Contents.ToArray()), out var kind))
                return // bugger. type system ahoy.
        }
    }
    class Stepper<T>
    {
        public Stepper(IEnumerable<T> enumerable)
        {
            enumerator = enumerable.GetEnumerator();
        }
        readonly IEnumerator<T> enumerator;

        public T NextOrDefault() => enumerator.MoveNext()
            ? enumerator.Current
            : default;
        public bool TryNext(out T value)
        {
            if (enumerator.MoveNext())
            {
                value = enumerator.Current;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public T Next() => TryNext(out var value)
            ? value
            : throw new InvalidOperationException("Sequence contains no more elements");
    }
    static class Expectation
    {
        public static Expectation<T> Valid<T>(T outcome) => Expectation<T>.Valid(outcome);
        public static Expectation<T> Invalid<T>(T outcome, string reason) => Expectation<T>.Invalid(outcome, reason);
    }
    struct Expectation<T>
    {
        public static Expectation<T> Valid(T outcome) => new Expectation<T>(outcome, null);
        public static Expectation<T> Invalid(T outcome, string reason)
        {
            if (reason == null)
                throw new ArgumentNullException(nameof(reason));
            return new Expectation<T>(outcome, reason);
        }
        Expectation(T outcome, string invalidReason)
        {
            Outcome = outcome;
            this.invalidReason = invalidReason;
        }
        public T Outcome { get; }
        public string InvalidReason => IsValid
            ? throw new InvalidOperationException("The outcome was successful")
            : invalidReason;
        bool IsValid => invalidReason == null;

        readonly string invalidReason;

        public Expectation<U> Then<U>(Func<T, Expectation<U>> projection, Func<T, U> failureProjection = null)
        {
            if (IsValid)
                return projection(Outcome);
            else
            {
                var outcome = failureProjection == null
                    ? default
                    : failureProjection(Outcome);
                return new Expectation<U>(outcome, InvalidReason);
            }
        }
    }

    enum TypeKind
    {
        Interface,
        Record,
        Enum
    }
}
