// <copyright file="DisconnectMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.InternalProtocol
{
    /// <summary>
    /// Internal Tempest protocol disconnect with reason message.
    /// </summary>
    public sealed class DisconnectMessage
        : TempestMessage
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DisconnectMessage"/>.
        /// </summary>
        public DisconnectMessage()
            : base(TempestMessageType.Disconnect)
        {
        }

        /// <summary>
        /// Gets or sets the reason for disconnection.
        /// </summary>
        public ConnectionResult Reason
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a custom reason for disconnection.
        /// </summary>
        public string CustomReason
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
            get { return true; }
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteByte((byte)Reason);
            if (Reason == ConnectionResult.Custom)
                writer.WriteString(CustomReason);
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            Reason = (ConnectionResult)reader.ReadByte();
            if (Reason == ConnectionResult.Custom)
                CustomReason = reader.ReadString();
        }
    }
}
