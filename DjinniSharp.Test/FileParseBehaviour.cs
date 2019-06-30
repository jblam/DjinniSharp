using DjinniSharp.Core;
using DjinniSharp.Core.File;
using DjinniSharp.Core.Parsing;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DjinniSharp.Test
{
    [TestFixture]
    public class FileParseBehaviour
    {
        [Test]
        public void ParsesFileWithTheLot()
        {
            var tokens = new DjinniLexer();
            var productions = new Parser();
            var file = new FileParser();

            var sections = file.Consume(
                productions.Consume(
                    tokens.Consume(
                        TestInputStrings.DirectiveAndTwoTypes)));

            CollectionAssert.AreEqual(
                new[]
                {
                    FileSection.Directives,
                    FileSection.Type,
                    FileSection.Type
                },
                sections.Select(s => s.Kind).Where(k => k != FileSection.Empty));
        }
    }
}
