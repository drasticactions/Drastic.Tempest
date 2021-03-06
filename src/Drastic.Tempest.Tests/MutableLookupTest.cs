using System;
using System.Linq;
using System.Collections.Generic;
using NUnit.Framework;

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class MutableLookupTest
    {
        internal MutableLookup<string, string> GetTestLookup()
        {
            var lookup = new MutableLookup<string, string>();
            lookup.Add(null, (string)null);
            lookup.Add(null, "blah");
            lookup.Add(null, "monkeys");
            lookup.Add("F", "Foo");
            lookup.Add("F", "Foobar");
            lookup.Add("B", "Bar");

            return lookup;
        }

        [TestCase(typeof(ArgumentNullException))]
        public void CtorNull(Type expectedException)
        {
            Assert.Throws(expectedException, () => { new MutableLookup<string, string>((ILookup<string, string>)null); });
        }

        [TestCase(typeof(ArgumentNullException))]
        public void CtorEqualityComparerNull(Type expectedException)
        {
            Assert.Throws(expectedException, () => new MutableLookup<string, string>((IEqualityComparer<string>)null));
        }

        [Test]
        public void CtorILookup()
        {
            List<int> ints = new List<int> { 10, 20, 21, 30 };
            var lookup = new MutableLookup<int, int>(ints.ToLookup(i => Int32.Parse(i.ToString()[0].ToString())));

            Assert.AreEqual(3, lookup.Count);
            Assert.AreEqual(1, lookup[1].Count());
            Assert.AreEqual(2, lookup[2].Count());
            Assert.AreEqual(1, lookup[3].Count());
        }

        [Test]
        public void CtorILookupWithNulls()
        {
            List<string> strs = new List<string> { "Foo", "Foos", "Foobar", "Monkeys", "Bar", "Ban", "Barfoo" };

            var lookup = new MutableLookup<string, string>(strs.ToLookup(s => (s[0] != 'F' && s[0] != 'B') ? null : s[0].ToString()));
            Assert.AreEqual(3, lookup.Count);
            Assert.AreEqual(3, lookup["F"].Count());
            Assert.AreEqual(3, lookup["B"].Count());
            Assert.AreEqual(1, lookup[null].Count());
        }

        [Test]
        public void Add()
        {
            var lookup = new MutableLookup<string, string>();
            lookup.Add("F", "Foo");
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual("Foo", lookup["F"].First());
            lookup.Add("F", "Foobar");
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual(2, lookup["F"].Count());
        }

        [Test]
        public void AddNull()
        {
            var lookup = new MutableLookup<string, string>();
            lookup.Add(null, "Foo");
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual(1, lookup[null].Count());
            lookup.Add(null, (string)null);
            Assert.AreEqual(1, lookup.Count);
            Assert.AreEqual(2, lookup[null].Count());
        }

        [Test]
        public void AddMultiple()
        {
            var values = new[] { "Foo", "Foobar" };
            var lookup = new MutableLookup<string, string>();
            lookup.Add("key", values);
            Assert.AreEqual(1, lookup.Count);
            CollectionAssert.Contains(lookup["key"], values[0]);
            CollectionAssert.Contains(lookup["key"], values[1]);
            lookup.Add("key2", values);
            Assert.AreEqual(2, lookup.Count);
            CollectionAssert.Contains(lookup["key2"], values[0]);
            CollectionAssert.Contains(lookup["key2"], values[1]);
        }

        [Test]
        public void AddMultipleNull()
        {
            var values = new[] { "Foo", "Foobar" };
            var lookup = new MutableLookup<string, string>();
            lookup.Add(null, values);
            Assert.AreEqual(1, lookup.Count);
            Assert.IsTrue(values.SequenceEqual(lookup[null]), "S");

            Assert.Throws<ArgumentNullException>(() => lookup.Add("foo", (IEnumerable<string>)null));
        }

        [Test]
        public void CountRefType()
        {
            var lookup = new MutableLookup<string, string>();
            Assert.AreEqual(0, lookup.Count);
            lookup.Add(null, "blah");
            Assert.AreEqual(1, lookup.Count);
            lookup.Add("F", "Foo");
            Assert.AreEqual(2, lookup.Count);
            lookup.Add("F", "Foobar");
            Assert.AreEqual(2, lookup.Count);

            lookup.Remove(null, "blah");
            Assert.AreEqual(1, lookup.Count);
        }

        [Test]
        public void CountValueType()
        {
            var lookup = new MutableLookup<int, int>();
            Assert.AreEqual(0, lookup.Count);
            lookup.Add(1, 10);
            Assert.AreEqual(1, lookup.Count);
            lookup.Add(2, 20);
            Assert.AreEqual(2, lookup.Count);
            lookup.Add(2, 21);
            Assert.AreEqual(2, lookup.Count);

            lookup.Remove(1, 10);
            Assert.AreEqual(1, lookup.Count);
        }

        [Test]
        public void RemoveExisting()
        {
            var lookup = new MutableLookup<string, string>();
            lookup.Add(null, "blah");
            lookup.Add(null, "monkeys");
            lookup.Add("F", "Foo");
            lookup.Add("F", "Foobar");
            lookup.Add("B", "Bar");

            Assert.AreEqual(3, lookup.Count);

            Assert.IsTrue(lookup.Remove(null, "blah"));
            Assert.AreEqual(3, lookup.Count);
            Assert.IsTrue(lookup.Remove(null, "monkeys"));
            Assert.AreEqual(2, lookup.Count);

            Assert.IsTrue(lookup.Remove("F"));
            Assert.AreEqual(1, lookup.Count);
        }

        [Test]
        public void RemoveNonExisting()
        {
            var lookup = new MutableLookup<string, string>();
            lookup.Add(null, "blah");
            lookup.Add(null, "monkeys");
            lookup.Add("F", "Foo");
            lookup.Add("F", "Foobar");
            lookup.Add("B", "Bar");

            Assert.IsFalse(lookup.Remove("D"));
            Assert.AreEqual(3, lookup.Count);

            Assert.IsFalse(lookup.Remove("F", "asdf"));
            Assert.AreEqual(3, lookup.Count);

            lookup.Remove(null);
            Assert.IsFalse(lookup.Remove(null));
            Assert.AreEqual(2, lookup.Count);
        }

        [Test]
        public void ClearWithNull()
        {
            var lookup = new MutableLookup<string, string>();
            lookup.Add(null, "blah");
            lookup.Add("F", "Foo");

            lookup.Clear();

            Assert.AreEqual(0, lookup.Count);
            Assert.IsFalse(lookup.Contains(null));
            Assert.IsFalse(lookup.Contains("F"));
        }

        [Test]
        public void ClearWithoutNull()
        {
            var lookup = new MutableLookup<string, string>();
            lookup.Add("F", "Foo");
            lookup.Add("F", "Foobar");
            lookup.Add("B", "Bar");

            lookup.Clear();
            Assert.AreEqual(0, lookup.Count);
            Assert.IsFalse(lookup.Contains("F"));
            Assert.IsFalse(lookup.Contains("B"));
        }

        [Test]
        public void ClearValueType()
        {
            var lookup = new MutableLookup<int, int>();
            lookup.Add(1, 10);
            lookup.Add(1, 12);
            lookup.Add(1, 13);
            lookup.Add(2, 21);
            lookup.Add(2, 22);
            lookup.Add(2, 23);

            lookup.Clear();
            Assert.AreEqual(0, lookup.Count);
            Assert.IsFalse(lookup.Contains(1));
            Assert.IsFalse(lookup.Contains(2));
        }

        [Test]
        public void Contains()
        {
            var lookup = new MutableLookup<string, string>();
            lookup.Add(null, "blah");
            lookup.Add(null, "monkeys");
            lookup.Add("F", "Foo");
            lookup.Add("F", "Foobar");
            lookup.Add("B", "Bar");

            Assert.IsTrue(lookup.Contains("B"));
        }

        [Test]
        public void DoesNotContain()
        {
            var lookup = new MutableLookup<string, string>();
            lookup.Add(null, "blah");
            lookup.Add(null, "monkeys");
            lookup.Add("F", "Foo");
            lookup.Add("F", "Foobar");
            lookup.Add("B", "Bar");

            Assert.IsFalse(lookup.Contains("D"));
        }

        [Test]
        public void ContainsNull()
        {
            var lookup = GetTestLookup();

            Assert.IsTrue(lookup.Contains(null));
        }

        [Test]
        public void DoesNotContainNull()
        {
            var lookup = new MutableLookup<string, string>();
            lookup.Add("F", "Foo");
            lookup.Add("F", "Foobar");
            lookup.Add("B", "Bar");

            Assert.IsFalse(lookup.Contains(null));
        }

        [Test]
        public void Indexer()
        {
            var lookup = GetTestLookup();

            Assert.AreEqual(2, lookup["F"].Count());
        }

        [Test]
        public void IndexerNull()
        {
            var lookup = GetTestLookup();

            Assert.AreEqual(3, lookup[null].Count());
        }

        [Test]
        public void IndexerNotFound()
        {
            var lookup = GetTestLookup();

            Assert.AreEqual(0, lookup["D"].Count());
        }

        [Test]
        public void Enumerator()
        {
            List<int> ints = new List<int> { 10, 20, 21, 30 };
            var lookup = new MutableLookup<int, int>(ints.ToLookup(i => Int32.Parse(i.ToString()[0].ToString())));

            Assert.AreEqual(3, lookup.Count());
            Assert.IsTrue(lookup.Any(g => g.Key == 1));
            Assert.IsTrue(lookup.Any(g => g.Key == 2));
            Assert.IsTrue(lookup.Any(g => g.Key == 3));
        }

        [Test]
        public void EnumeratorNotNull()
        {
            List<string> strings = new List<string> { "hi", "hai", "bai", "bye" };
            var lookup = new MutableLookup<string, string>(strings.ToLookup(s => s[0].ToString()));

            Assert.AreEqual(2, lookup.Count);
            Assert.IsTrue(lookup.Any(g => g.Key == "h"));
            Assert.IsTrue(lookup.Any(g => g.Key == "b"));
            Assert.IsFalse(lookup.Any(g => g.Key == null));
        }

        [Test]
        public void EnumeratorNull()
        {
            var lookup = GetTestLookup();

            Assert.AreEqual(3, lookup.Count());
            Assert.IsTrue(lookup.Any(g => g.Key == null));
            Assert.IsTrue(lookup.Any(g => g.Key == "F"));
            Assert.IsTrue(lookup.Any(g => g.Key == "B"));
        }

        [Test]
        public void NullGroupingEnumerator()
        {
            var lookup = GetTestLookup();

            Assert.AreEqual(3, lookup[null].Count());
            Assert.IsTrue(lookup[null].Any(s => s == "blah"));
            Assert.IsTrue(lookup[null].Any(s => s == "monkeys"));
            Assert.IsTrue(lookup[null].Any(s => s == null));
        }

        [Test]
        public void GroupingEnumerator()
        {
            List<int> ints = new List<int> { 10, 20, 21, 30 };
            var lookup = new MutableLookup<int, int>(ints.ToLookup(i => Int32.Parse(i.ToString()[0].ToString())));

            Assert.AreEqual(2, lookup[2].Count());
            Assert.IsTrue(lookup[2].Any(i => i == 20));
            Assert.IsTrue(lookup[2].Any(i => i == 21));
        }

        [Test]
        public void TryGetValuesNull()
        {
            var lookup = GetTestLookup();

            IEnumerable<string> values;
            Assert.IsTrue(lookup.TryGetValues(null, out values));
            Assert.IsNotNull(values);

            var v = values.ToList();
            Assert.AreEqual(3, v.Count);
            CollectionAssert.Contains(v, "blah");
            CollectionAssert.Contains(v, "monkeys");
            Assert.Contains("blah", v);
            Assert.Contains("monkeys", v);
            Assert.Contains(null, v);
        }

        [Test]
        public void TryGetValues()
        {
            var lookup = GetTestLookup();

            IEnumerable<string> values;
            Assert.IsTrue(lookup.TryGetValues("F", out values));
            Assert.IsNotNull(values);

            var v = values.ToList();
            Assert.AreEqual(2, v.Count);
            Assert.Contains("Foo", v);
            Assert.Contains("Foobar", v);
        }

        [Test]
        public void TryGetValuesFail()
        {
            var lookup = GetTestLookup();

            IEnumerable<string> values;
            Assert.IsFalse(lookup.TryGetValues("notfound", out values));
            Assert.IsNull(values);
        }

        [Test]
        public void TryGetValuesNullFail()
        {
            var lookup = new MutableLookup<string, string>();

            IEnumerable<string> values;
            Assert.IsFalse(lookup.TryGetValues(null, out values));
            Assert.IsNull(values);
        }
    }
}
