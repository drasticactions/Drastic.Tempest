// <copyright file="MockProtocol.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public class MockProtocol
    {
        public static Protocol Instance
        {
            get { return p; }
        }

        private static readonly Protocol p;
        static MockProtocol()
        {
            p = new Protocol(2);
            p.Register(new[]
            {
                new KeyValuePair<Type, Func<Message>> (typeof (MockMessage), () => new MockMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof (BlankMessage), () => new BlankMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof (AuthenticatedMessage), () => new AuthenticatedMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof (EncryptedMessage), () => new EncryptedMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof (AuthenticatedAndEncryptedMessage), () => new AuthenticatedAndEncryptedMessage()),
            });
        }
    }
}
