// <copyright file="ContentMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public abstract class ContentMessage
            : Message
    {
        protected ContentMessage(ushort id)
            : base(MockProtocol.Instance, id)
        {
        }

        public string Message
        {
            get;
            set;
        }

        public int Number
        {
            get;
            set;
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteString(Message);
            writer.WriteInt32(Number);
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            Message = reader.ReadString();
            Number = reader.ReadInt32();
        }
    }
}
