// <copyright file="PingMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.InternalProtocol
{
    /// <summary>
    /// Internal Tempest protocol ping message.
    /// </summary>
    public sealed class PingMessage
        : TempestMessage
    {
        public PingMessage()
            : base(TempestMessageType.Ping)
        {
        }

        /// <summary>
        /// Gets or sets the ping interval.
        /// </summary>
        public int Interval
        {
            get;
            set;
        }

        public override bool MustBeReliable
        {
            get { return true; }
        }

        public override bool PreferReliable
        {
            get { return true; }
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteInt32(Interval);
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            Interval = reader.ReadInt32();
        }
    }
}
