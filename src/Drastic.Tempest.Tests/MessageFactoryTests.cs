// <copyright file="MessageFactoryTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Reflection;

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class MessageFactoryTests
    {
        private static Protocol protocol;

        [SetUp]
        public void Setup()
        {
            protocol = new Protocol(MockProtocol.Instance.id, MockProtocol.Instance.Version);
        }

        [Test]
        public void DiscoverNull()
        {
            Assert.Throws<ArgumentNullException>(() => protocol.Discover(null));
        }

        [Test]
        public void Discover()
        {
            protocol.Discover();

            Message m = protocol.Create(1);
            Assert.IsNotNull(m);
            Assert.That(m, Is.TypeOf<MockMessage>());
        }

        [Test]
        public void DiscoverAssembly()
        {
            protocol.Discover(typeof(MessageFactoryTests).GetTypeInfo().Assembly);

            Message m = protocol.Create(1);
            Assert.IsNotNull(m);
            Assert.That(m, Is.TypeOf<MockMessage>());
        }

        [Test]
        public void DiscoverFromAssymblyOf()
        {
            protocol.DiscoverFromAssemblyOf<MessageFactoryTests>();

            Message m = protocol.Create(1);
            Assert.IsNotNull(m);
            Assert.That(m, Is.TypeOf<MockMessage>());
        }

        [Test]
        public void DiscoverAssemblyNothing()
        {
            protocol.Discover(typeof(string).GetTypeInfo().Assembly);

            Message m = protocol.Create(1);
            Assert.IsNull(m);
        }

        [Test]
        public void RegisterNull()
        {
            Assert.Throws<ArgumentNullException>(() => protocol.Register((IEnumerable<KeyValuePair<Type, Func<Message>>>)null));
        }

        private class PrivateMessage
            : Message
        {
            public PrivateMessage(Protocol protocol, ushort type)
                : base(protocol, type)
            {
            }

            public override void WritePayload(ISerializationContext context, IValueWriter writer)
            {
            }

            public override void ReadPayload(ISerializationContext context, IValueReader reader)
            {
            }
        }

        private class GenericMessage<T>
            : Message
        {
            public GenericMessage()
                : base(MockProtocol.Instance, 3)
            {
            }

            public T Element
            {
                get;
                set;
            }

            public override void WritePayload(ISerializationContext context, IValueWriter writer)
            {
                writer.Write(context, Element);
            }

            public override void ReadPayload(ISerializationContext context, IValueReader reader)
            {
                Element = reader.Read<T>(context);
            }
        }


        [Test]
        public void RegisterTypeAndCtorsInvalid()
        {
            Assert.Throws<ArgumentException>(() =>
                protocol.Register(new[] { new KeyValuePair<Type, Func<Message>>(typeof(string), () => new MockMessage()) }));
            Assert.Throws<ArgumentException>(() =>
                protocol.Register(new[] { new KeyValuePair<Type, Func<Message>>(typeof(int), () => new MockMessage()) }));
        }

        [Test]
        public void RegisterTypeAndCtorsDuplicates()
        {
            Assert.Throws<ArgumentException>(() =>
                protocol.Register(new[]
                {
                    new KeyValuePair<Type, Func<Message>> (typeof (MockMessage), () => new MockMessage ()),
                    new KeyValuePair<Type, Func<Message>> (typeof (MockMessage), () => new MockMessage ()),
                }));
        }

        [Test]
        public void RegisterTypeWithCtor()
        {
            protocol.Register(new[]
            {
                new KeyValuePair<Type, Func<Message>> (typeof(MockMessage), () => new MockMessage ()),
                new KeyValuePair<Type, Func<Message>> (typeof(PrivateMessage), () => new PrivateMessage (protocol, 2)),
                new KeyValuePair<Type, Func<Message>> (typeof(PrivateMessage), () => new PrivateMessage (protocol, 3)),
            });

            Message m = protocol.Create(1);
            Assert.IsNotNull(m);
            Assert.That(m, Is.TypeOf<MockMessage>());

            m = protocol.Create(2);
            Assert.IsNotNull(m);
            Assert.AreEqual(2, m.MessageType);
            Assert.That(m, Is.TypeOf<PrivateMessage>());

            m = protocol.Create(3);
            Assert.IsNotNull(m);
            Assert.AreEqual(3, m.MessageType);
            Assert.That(m, Is.TypeOf<PrivateMessage>());
        }
    }
}
