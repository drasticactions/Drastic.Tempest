// <copyright file="StreamReaderWriterTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class StreamReaderWriterTests
        : ReaderWriterPairTests
    {
        protected override IValueWriter GetWriter()
        {
            return new StreamValueWriter(new ResettingMemoryStream(20480));
        }

        protected override IValueReader GetReader(IValueWriter writer)
        {
            StreamValueWriter streamWriter = (StreamValueWriter)writer;

            return new StreamValueReader(streamWriter.stream);
        }

        [Test]
        public void WriterCtorNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StreamValueWriter(null));
        }

        [Test]
        public void ReaderCtorNull()
        {
            Assert.Throws<ArgumentNullException>(() => new StreamValueReader(null));
        }
    }
}
