// <copyright file="TempestServerTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class TempestServerTests
    {
        private static readonly Protocol protocol;
        static TempestServerTests()
        {
            protocol = ProtocolTests.GetTestProtocol();
        }

        private MockConnectionProvider provider;

        [SetUp]
        public void Setup()
        {
            provider = new MockConnectionProvider(protocol);
        }

        [Test]
        public void ProviderCtorNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TempestServer(null, MessageTypes.Reliable));
        }

        [Test]
        public void AddConnectionProviderNull()
        {
            var server = new TempestServer(provider, MessageTypes.Reliable);
            Assert.Throws<ArgumentNullException>(() => server.AddConnectionProvider(null));
        }

        [Test]
        public void RemoveConnectionProviderNull()
        {
            var server = new TempestServer(provider, MessageTypes.Reliable);
            Assert.Throws<ArgumentNullException>(() => server.RemoveConnectionProvider(null));
        }

        [Test]
        public void AddConnectionProvider()
        {
            var p = new MockConnectionProvider(protocol);
            Assert.IsFalse(p.IsRunning);

            var server = new TempestServer(provider, MessageTypes.Reliable);
            server.AddConnectionProvider(p);
            Assert.IsFalse(p.IsRunning);
        }

        [Test]
        public void AddAfterStarting()
        {
            var p = new MockConnectionProvider(protocol);
            Assert.IsFalse(p.IsRunning);

            var server = new TempestServer(provider, MessageTypes.Reliable);
            server.Start();

            server.AddConnectionProvider(p);
            Assert.IsTrue(p.IsRunning);
        }

        [Test]
        public void RemoveConnectionProvider()
        {
            var server = new TempestServer(provider, MessageTypes.Reliable);
            Assert.IsFalse(provider.IsRunning);

            server.RemoveConnectionProvider(provider);
            Assert.IsFalse(provider.IsRunning);
        }

        [Test]
        public void RemoveConnectionProviderGlobalOrder()
        {
            var server = new TempestServer(MessageTypes.Reliable);
            server.AddConnectionProvider(provider, ExecutionMode.GlobalOrder);
            Assert.IsFalse(provider.IsRunning);

            server.RemoveConnectionProvider(provider);
            Assert.IsFalse(provider.IsRunning);
        }

        [Test]
        public void RemoveNotAddedConnectionProvider()
        {
            var p = new MockConnectionProvider(protocol);
            p.Start(MessageTypes.Reliable);
            Assert.IsTrue(p.IsRunning);

            var server = new TempestServer(provider, MessageTypes.Reliable);
            server.RemoveConnectionProvider(p);
            Assert.IsTrue(p.IsRunning);
        }

        [Test]
        public void MessageHandling()
        {
            IServerConnection connection = null;

            var test = new AsyncTest(e =>
            {
                var me = (MessageEventArgs)e;
                Assert.AreSame(connection, me.Connection);
                Assert.IsTrue(me.Message is MockMessage);
                Assert.AreEqual("hi", ((MockMessage)me.Message).Content);
            });

            var server = new TempestServer(provider, MessageTypes.Reliable);
            server.Start();

            Action<MessageEventArgs> handler = e => test.PassHandler(test, e);
            ((IContext)server).RegisterMessageHandler(MockProtocol.Instance, 1, handler);

            provider.ConnectionMade += (sender, e) => connection = e.Connection;

            var c = provider.GetClientConnection(protocol);
            c.ConnectAsync(new Target(Target.AnyIP, 0), MessageTypes.Reliable);
            c.SendAsync(new MockMessage() { Content = "hi" });

            test.Assert(10000);
        }

        [Test]
        public void GenericMessageHandling()
        {
            IServerConnection connection = null;

            var test = new AsyncTest(e =>
            {
                var me = (MessageEventArgs<MockMessage>)e;
                Assert.AreSame(connection, me.Connection);
                Assert.AreEqual("hi", me.Message.Content);
            });

            var server = new TempestServer(provider, MessageTypes.Reliable);
            server.Start();

            Action<MessageEventArgs<MockMessage>> handler = e => test.PassHandler(test, e);
            server.RegisterMessageHandler(handler);

            provider.ConnectionMade += (sender, e) => connection = e.Connection;

            var c = provider.GetClientConnection(protocol);
            c.ConnectAsync(new Target(Target.AnyIP, 0), MessageTypes.Reliable);
            c.SendAsync(new MockMessage { Content = "hi" });

            test.Assert(10000);
        }

        [Test]
        public void MessageHandlingGlobalOrder()
        {
            IServerConnection connection = null;

            var test = new AsyncTest(e =>
            {
                var me = (MessageEventArgs)e;
                Assert.AreSame(connection, me.Connection);
                Assert.IsTrue(me.Message is MockMessage);
                Assert.AreEqual("hi", ((MockMessage)me.Message).Content);
            });

            var server = new TempestServer(MessageTypes.Reliable);
            server.AddConnectionProvider(provider, ExecutionMode.GlobalOrder);
            server.Start();

            Action<MessageEventArgs> handler = e => test.PassHandler(test, e);
            ((IContext)server).RegisterMessageHandler(MockProtocol.Instance, 1, handler);

            provider.ConnectionMade += (sender, e) => connection = e.Connection;

            var c = provider.GetClientConnection(protocol);
            c.ConnectAsync(new Target(Target.AnyIP, 0), MessageTypes.Reliable);
            c.SendAsync(new MockMessage { Content = "hi" });

            test.Assert(10000);
        }

        [Test]
        public void GenericMessageHandlingGlobalOrder()
        {
            IServerConnection connection = null;

            var test = new AsyncTest(e =>
            {
                var me = (MessageEventArgs<MockMessage>)e;
                Assert.AreSame(connection, me.Connection);
                Assert.AreEqual("hi", me.Message.Content);
            });

            var server = new TempestServer(MessageTypes.Reliable);
            server.AddConnectionProvider(provider, ExecutionMode.GlobalOrder);
            server.Start();

            Action<MessageEventArgs<MockMessage>> handler = e => test.PassHandler(test, e);
            server.RegisterMessageHandler(handler);

            provider.ConnectionMade += (sender, e) => connection = e.Connection;

            var c = provider.GetClientConnection(protocol);
            c.ConnectAsync(new Target(Target.AnyIP, 0), MessageTypes.Reliable);
            c.SendAsync(new MockMessage { Content = "hi" });

            test.Assert(10000);
        }

        [Test]
        public void ConnectionMade()
        {
            var server = new TempestServer(provider, MessageTypes.Reliable);
            server.Start();

            var test = new AsyncTest();
            server.ConnectionMade += test.PassHandler;

            var client = provider.GetClientConnection(protocol);
            client.ConnectAsync(new Target(Target.AnyIP, 0), MessageTypes.Reliable);

            test.Assert(5000);
        }

        [Test]
        public void ConnectionMadeGlobalOrder()
        {
            var server = new TempestServer(MessageTypes.Reliable);
            server.AddConnectionProvider(provider, ExecutionMode.GlobalOrder);
            server.Start();

            var test = new AsyncTest();
            server.ConnectionMade += test.PassHandler;

            var client = provider.GetClientConnection(protocol);
            client.ConnectAsync(new Target(Target.AnyIP, 0), MessageTypes.Reliable);

            test.Assert(5000);
        }
    }
}
