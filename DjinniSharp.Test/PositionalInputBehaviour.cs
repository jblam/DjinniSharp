using DjinniSharp.Core;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DjinniSharp.Test
{
    [TestFixture]
    public class PositionalInputBehaviour
    {
        [Test]
        public void CanEnumerateString()
        {
            var sut = new StringInput("asdf");
            CollectionAssert.AreEqual("asdf".ToCharArray(), sut.GetEnumerable(0));
        }

        [Test]
        public void CanEnumerateNested()
        {
            const string input = "asdf";
            var sut = new StringInput(input);
            int visitedItems = 0;
            foreach (var outerItem in sut.GetEnumerable(0))
            {
                var innerItem = sut.GetEnumerable(visitedItems, 1).ToList().Single();
                Assert.AreEqual(outerItem, innerItem);
                visitedItems += 1;
            }
            Assert.AreEqual(input.Length, visitedItems);
        }

        static Stream FromString(string input) => new MemoryStream(Encoding.UTF8.GetBytes(input));

        [Test]
        public void CanEnumerateStream()
        {
            const string input = "asdf";
            var sut = new StreamInput(FromString(input), Encoding.UTF8);
            CollectionAssert.AreEqual(input.ToCharArray(), sut.GetEnumerable(0));
        }

        [Test]
        public void CanEnumerateStreamNested()
        {
            const string input = "asdf";
            var sut = new StreamInput(FromString(input), Encoding.UTF8);
            int visitedItems = 0;
            foreach (var outerItem in sut.GetEnumerable(0))
            {
                var innerItem = sut.GetEnumerable(visitedItems, 1).ToList().Single();
                Assert.AreEqual(outerItem, innerItem);
                visitedItems += 1;
            }
            Assert.AreEqual(input.Length, visitedItems);
        }
    }
}
