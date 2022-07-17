// <copyright file="ExtensionsTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        public void ReadWriteUniversalDate()
        {
            byte[] buffer = new byte[20480];
            var writer = new BufferValueWriter(buffer);

            DateTime d = DateTime.Now;

            writer.WriteUniversalDate(d);
            writer.Flush();

            var reader = new BufferValueReader(buffer);

            Assert.AreEqual(d.ToUniversalTime(), reader.ReadUniversalDate());
        }

        [Test]
        public void ReadWrite7BitInt()
        {
            var writer = new BufferValueWriter(new byte[20480]);

            writer.Write7BitEncodedInt(Int32.MinValue);
            writer.Write7BitEncodedInt(0);
            writer.Write7BitEncodedInt(Int32.MaxValue);
            writer.Flush();

            var reader = new BufferValueReader(writer.Buffer);

            Assert.AreEqual(Int32.MinValue, reader.Read7BitEncodedInt());
            Assert.AreEqual(0, reader.Read7BitEncodedInt());
            Assert.AreEqual(Int32.MaxValue, reader.Read7BitEncodedInt());
        }
    }
}
