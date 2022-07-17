// <copyright file="MockProtocol2.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public class MockProtocol2
    {
        public static Protocol Instance
        {
            get { return p; }
        }

        private static readonly Protocol p;

        static MockProtocol2()
        {
            p = new Protocol(3);
            p.Register(new[]
            {
                new KeyValuePair<Type, Func<Message>> (typeof (MockMessage2), () => new MockMessage2())
            });
        }
    }
}
