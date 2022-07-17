// <copyright file="PartialMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.InternalProtocol
{
    public class PartialMessage
            : TempestMessage
    {
        public PartialMessage()
            : base(TempestMessageType.Partial)
        {
        }

        public ushort OriginalMessageId
        {
            get;
            set;
        }

        public byte Count
        {
            get;
            set;
        }

        public byte[] Payload
        {
            get { return this.payload; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                this.payload = value;
                this.offset = 0;
                this.length = value.Length;
            }
        }

        public void SetPayload(byte[] msgPayload, int payloadOffset, int payloadLength)
        {
            if (msgPayload == null)
                throw new ArgumentNullException("msgPayload");

            this.payload = msgPayload;
            this.offset = payloadOffset;
            this.length = payloadLength;
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteUInt16(OriginalMessageId);
            writer.WriteByte(Count);
            writer.WriteBytes(Payload, this.offset, this.length);
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            OriginalMessageId = reader.ReadUInt16();
            Count = reader.ReadByte();
            Payload = reader.ReadBytes();
        }

        private int offset;
        private int length;
        private byte[] payload;
    }
}
