// <copyright file="BufferPoolTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Net.Sockets;
using Drastic.Tempest.Providers.Network;

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class BufferPoolTests
    {
        [Test]
        public void CtorInvalid()
        {
            Assert.That(() => new BufferPool(0), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => new BufferPool(0, 1), Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => new BufferPool(1, 0), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void Ctor()
        {
            var buffer = new BufferPool(128, 2);
            Assert.AreEqual(2, buffer.Limit);
        }

        [Test]
        public void AddConnection()
        {
            var buffer = new BufferPool(128);
            buffer.AutoSizeFactor = 2;
            buffer.AutoSizeLimit = true;

            buffer.AddConnection();
            Assert.AreEqual(2, buffer.Limit);
        }

        [Test]
        public void AddConnectionNoAutosizing()
        {
            var buffer = new BufferPool(128);
            buffer.AutoSizeFactor = 2;
            buffer.AutoSizeLimit = false;

            buffer.AddConnection();
            Assert.AreEqual(0, buffer.Limit);
        }

        [Test]
        public void PushNull()
        {
            var buffer = new BufferPool(128);
            Assert.That(() => buffer.PushBuffer(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TryGetBufferNew()
        {
            var buffer = new BufferPool(128, 2);

            SocketAsyncEventArgs e;
            Assert.IsFalse(buffer.TryGetBuffer(out e), "TryGetBuffer did not signal a newly created buffer");
            Assert.IsNotNull(e);
            Assert.AreEqual(e.Buffer.Length, 128, "Buffer length was not the default size");
        }

        [Test]
        public void TryGetBufferBlocksAndExisting()
        {
            var buffer = new BufferPool(128, 1);

            SocketAsyncEventArgs e;
            buffer.TryGetBuffer(out e);

            DateTime now = DateTime.Now;

            SocketAsyncEventArgs second = null;
            var test = new AsyncTest(args => {
                Assert.That(DateTime.Now - now, Is.GreaterThan(TimeSpan.FromSeconds(1)));
                Assert.That(second, Is.SameAs(e));
            });

            Task.Run(() => {
                Assert.IsTrue(buffer.TryGetBuffer(out second));
            }).ContinueWith(t => {
                test.PassHandler(null, EventArgs.Empty);
            });

            Task.Delay(1000).ContinueWith(t => {
                buffer.PushBuffer(e);
            });

            test.Assert(2000);
        }

        [Test]
        [Description("If the buffer limit has dropped below the max, pushing buffers back should destroy them")]
        public void PushBufferDestroys()
        {
            var buffer = new BufferPool(128)
            {
                AutoSizeLimit = true,
                AutoSizeFactor = 1
            };

            buffer.AddConnection();

            SocketAsyncEventArgs e;
            buffer.TryGetBuffer(out e);

            buffer.RemoveConnection();

            buffer.PushBuffer(e);

            Assert.That(() => e.SetBuffer(0, 128), Throws.TypeOf<ObjectDisposedException>());
        }
    }
}
