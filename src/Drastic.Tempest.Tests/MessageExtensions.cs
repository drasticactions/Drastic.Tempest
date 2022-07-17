// <copyright file="MessageExtensions.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Tempest.Tests
{
    public static class MessageExtensions
    {
        public static T AssertLengthMatches<T>(this T self)
            where T : Message, new()
        {
            if (self == null)
                throw new ArgumentNullException("self");

            var stream = new MemoryStream(20480);
            var writer = new StreamValueWriter(stream);
            var reader = new StreamValueReader(stream);

            var msg = new T();
            var context = SerializationContextTests.GetContext(msg.Protocol);

            self.WritePayload(context, writer);
            long len = stream.Position;
            stream.Position = 0;

            msg.ReadPayload(context, reader);
            Assert.AreEqual(len, stream.Position);

            return msg;
        }
    }
}
