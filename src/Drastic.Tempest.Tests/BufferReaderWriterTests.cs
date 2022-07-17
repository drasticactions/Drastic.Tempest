// <copyright file="BufferReaderWriterTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class BufferReaderWriterTests
            : ReaderWriterPairTests
    {
        protected override IValueWriter GetWriter()
        {
            return new BufferValueWriter(new byte[20480]);
        }

        protected override IValueReader GetReader(IValueWriter writer)
        {
            BufferValueWriter bufferWriter = (BufferValueWriter)writer;
            return new BufferValueReader(bufferWriter.Buffer);
        }

        [Test]
        public void ReaderCtorNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BufferValueReader(null));
        }

        [Test]
        public void WriterCtorNull()
        {
            Assert.Throws<ArgumentNullException>(() => new BufferValueWriter(null));
        }

        [Test]
        public void BufferOverflowResize()
        {
            byte[] buffer = new byte[4];
            var writer = new BufferValueWriter(buffer);
            writer.WriteInt64(1);

            Assert.That(writer.Length, Is.EqualTo(8));
            Assert.That(writer.Buffer.Length, Is.AtLeast(8));
        }

        [Test]
        public void ReadWriteLongSet()
        {
            var writer = new BufferValueWriter(new byte[1]);

            for (int i = 0; i < 20480; ++i)
                writer.WriteInt32(i);

            writer.Flush();

            var reader = new BufferValueReader(writer.Buffer);
            for (int i = 0; i < 20480; ++i)
                Assert.AreEqual(i, reader.ReadInt32());
        }
    }
}
