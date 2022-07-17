// <copyright file="UnreliableMockMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Text;

namespace Drastic.Tempest.Tests
{
    public class UnreliableMockMessage
            : Message
    {
        public UnreliableMockMessage()
            : base(MockProtocol.Instance, 9)
        {
        }

        public override bool MustBeReliable
        {
            get { return false; }
        }

        public override bool AcceptedConnectionlessly
        {
            get { return true; }
        }

        public override bool PreferReliable
        {
            get { return false; }
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
