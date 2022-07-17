// <copyright file="MockConnectionProviderTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class MockConnectionProviderTests
            : ConnectionProviderTests
    {
        private Protocol p = MockProtocol.Instance;

        protected override Target Target
        {
            get { return new Target(Target.AnyIP, 0); }
        }

        protected override MessageTypes MessageTypes
        {
            get { return MessageTypes.Reliable; }
        }

        protected override IConnectionProvider SetUp()
        {
            return new MockConnectionProvider(p);
        }

        protected override IConnectionProvider SetUp(IEnumerable<Protocol> protocols)
        {
            return new MockConnectionProvider(protocols);
        }

        protected override IClientConnection SetupClientConnection()
        {
            return ((MockConnectionProvider)this.provider).GetClientConnection();
        }

        protected override IClientConnection SetupClientConnection(IEnumerable<Protocol> protocols)
        {
            return ((MockConnectionProvider)this.provider).GetClientConnection(protocols);
        }

        [Test]
        public void ServerConnectionConnected()
        {
            var provider = new MockConnectionProvider(MockProtocol.Instance);
            provider.Start(MessageTypes.Reliable);

            var test = new AsyncTest<ConnectionMadeEventArgs>(e => Assert.AreEqual(true, e.Connection.IsConnected));
            provider.ConnectionMade += test.PassHandler;

            var client = provider.GetClientConnection(MockProtocol.Instance);
            client.ConnectAsync(new Target(Target.AnyIP, 0), MessageTypes.Reliable);

            test.Assert(5000);
        }
    }
}
