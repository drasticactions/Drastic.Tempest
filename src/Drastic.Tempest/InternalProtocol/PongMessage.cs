// <copyright file="PongMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.InternalProtocol
{
    /// <summary>
    /// Internal Tempest protocol pong message.
    /// </summary>
    public sealed class PongMessage
        : TempestMessage
    {
        public PongMessage()
            : base(TempestMessageType.Pong)
        {
        }

        public override bool MustBeReliable
        {
            get { return true; }
        }

        public override bool PreferReliable
        {
            get { return true; }
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
        }
    }
}
