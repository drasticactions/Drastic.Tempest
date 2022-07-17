// <copyright file="MockMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Tempest.Tests
{
    public class MockMessage
        : Message
    {
        public MockMessage()
            : base(MockProtocol.Instance, 1)
        {
        }

        public string Content
        {
            get; set;
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteString(Encoding.UTF8, Content);
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            Content = reader.ReadString(Encoding.UTF8);
        }
    }
}
