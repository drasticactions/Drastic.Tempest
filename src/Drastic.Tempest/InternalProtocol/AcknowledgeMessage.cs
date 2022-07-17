// <copyright file="AcknowledgeMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.InternalProtocol
{
    internal class AcknowledgeMessage
            : TempestMessage
    {
        public AcknowledgeMessage()
            : base(TempestMessageType.Acknowledge)
        {
        }

        public int[] MessageIds
        {
            get;
            set;
        }

        public override bool MustBeReliable
        {
            get { return false; }
        }

        public override bool PreferReliable
        {
            get { return false; }
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteInt32(MessageIds.Length);
            for (int i = 0; i < MessageIds.Length; i++)
                writer.WriteInt32(MessageIds[i]);
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            MessageIds = new int[reader.ReadInt32()];
            for (int i = 0; i < MessageIds.Length; i++)
                MessageIds[i] = reader.ReadInt32();
        }
    }
}
