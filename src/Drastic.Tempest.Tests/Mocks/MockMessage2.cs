// <copyright file="MockMessage2.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Text;

namespace Drastic.Tempest.Tests
{
    public class MockMessage2
        : Message
    {
        public MockMessage2()
            : base(MockProtocol2.Instance, 1)
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
