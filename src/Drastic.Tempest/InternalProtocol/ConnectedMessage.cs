// <copyright file="ConnectedMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.InternalProtocol
{
    public sealed class ConnectedMessage
        : TempestMessage
    {
        public ConnectedMessage()
            : base(TempestMessageType.Connected)
        {
        }

        public int ConnectionId
        {
            get;
            set;
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteInt32(ConnectionId);
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            ConnectionId = reader.ReadInt32();
        }
    }
}
