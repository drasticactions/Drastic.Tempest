// <copyright file="ContextTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public abstract class ContextTests
    {
        protected abstract IContext Client
        {
            get;
        }

        [Test]
        public void RegisterHandler_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Client.RegisterMessageHandler(null, 1, args => { }));
            Assert.Throws<ArgumentNullException>(() => Client.RegisterMessageHandler(MockProtocol.Instance, 1, null));
        }

        [Test]
        public void RegisterHandler_Locked()
        {
            Client.LockHandlers();

            Assert.Throws<InvalidOperationException>(() =>
            {
                Action<MessageEventArgs> handler = e => { };
                Client.RegisterMessageHandler(MockProtocol.Instance, 1, handler);
            });
        }

        [Test]
        public void RegisterConnectionlessHandler_Invalid()
        {
            Assert.Throws<ArgumentNullException>(() => Client.RegisterConnectionlessMessageHandler(null, 1, args => { }));
            Assert.Throws<ArgumentNullException>(() => Client.RegisterConnectionlessMessageHandler(MockProtocol.Instance, 1, null));
        }

        [Test]
        public void RegisterConnetionlessHandler_Locked()
        {
            Client.LockHandlers();

            Assert.Throws<InvalidOperationException>(() =>
            {
                Action<ConnectionlessMessageEventArgs> handler = e => { };
                Client.RegisterConnectionlessMessageHandler(MockProtocol.Instance, 1, handler);
            });
        }
    }
}
