// <copyright file="SerializationContextTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class SerializationContextTests
    {
        public static ISerializationContext GetContext(Protocol protocol)
        {
            return new SerializationContext(new MockClientConnection(new MockConnectionProvider(protocol)),
                new Dictionary<byte, Protocol> { { protocol.id, protocol } });
        }

        [Test]
        public void CtorNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SerializationContext(null));
            Assert.Throws<ArgumentNullException>(() => new SerializationContext(null, new Dictionary<byte, Protocol>()));
            Assert.Throws<ArgumentNullException>(() => new SerializationContext(new MockClientConnection(new MockConnectionProvider(MockProtocol.Instance)), null));
        }

        [Test]
        public void Ctor()
        {
            var c = new MockClientConnection(new MockConnectionProvider(MockProtocol.Instance));

            var protocols = new Dictionary<byte, Protocol> {
                { MockProtocol.Instance.id, MockProtocol.Instance }
            };

            var context = new SerializationContext(c, protocols);
            Assert.AreSame(c, context.Connection);
            Assert.AreSame(MockProtocol.Instance, context.Protocols.First().Value);
        }
    }
}
