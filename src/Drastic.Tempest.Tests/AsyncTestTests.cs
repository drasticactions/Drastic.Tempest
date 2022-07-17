// <copyright file="AsyncTestTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class AsyncTestTests
    {
        [Test]
        public void NonGeneric_Ctor_Invalid()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncTest(-1));
            Assert.Throws<ArgumentNullException>(() => new AsyncTest((Action<EventArgs>)null));
            Assert.Throws<ArgumentNullException>(() => new AsyncTest((Action<EventArgs>)null, multiple: true));
            Assert.Throws<ArgumentNullException>(() => new AsyncTest((Action<EventArgs>)null, times: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncTest(e => Assert.Pass(), times: -1));
            Assert.Throws<ArgumentNullException>(() => new AsyncTest((Func<EventArgs, bool>)null));
            Assert.Throws<ArgumentNullException>(() => new AsyncTest((Func<EventArgs, bool>)null, multiple: true));
            Assert.Throws<ArgumentNullException>(() => new AsyncTest((Func<EventArgs, bool>)null, times: 2));
            Assert.Throws<ArgumentOutOfRangeException>(() => new AsyncTest(e => true, times: -1));
        }

        [Test]
        public void FailIfNotPassed()
        {
            var test = new AsyncTest();

            Assert.Throws<AssertionException>(() => test.Assert(10, failIfNotPassed: true));
        }

        [Test]
        public void FailIfNotPassed_False()
        {
            var test = new AsyncTest();

            test.Assert(10, failIfNotPassed: false);
        }

        [Test]
        public void Multiple_Passes_NoFailures()
        {
            var test = new AsyncTest(multiple: true);
            test.PassHandler(this, EventArgs.Empty);
            test.PassHandler(this, EventArgs.Empty);

            test.Assert(10, failIfNotPassed: false);
        }

        [Test]
        public void Multiple_Passes_FailIfNotPassed()
        {
            var test = new AsyncTest(multiple: true);
            test.PassHandler(this, EventArgs.Empty);
            // failIfNotPassed refers to explicit Assert.Pass() calls only, not PassHandler calls.
            Assert.Throws<AssertionException>(() => test.Assert(10, failIfNotPassed: true));
        }

        [Test]
        public void Multiple_Passes_Failure()
        {
            var test = new AsyncTest(multiple: true);
            test.PassHandler(this, EventArgs.Empty);
            test.PassHandler(this, EventArgs.Empty);
            test.FailHandler(this, EventArgs.Empty);

            Assert.Throws<AssertionException>(() => test.Assert(10, failIfNotPassed: false));
        }
    }
}
