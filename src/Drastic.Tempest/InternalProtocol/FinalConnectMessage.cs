// <copyright file="FinalConnectMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.InternalProtocol
{
    public sealed class FinalConnectMessage
            : TempestMessage
    {
        public FinalConnectMessage()
            : base(TempestMessageType.FinalConnect)
        {
        }

        public override bool Authenticated
        {
            get { return true; }
        }

        public override bool PreferReliable
        {
            get { return true; }
        }

        public byte[] AESKey
        {
            get;
            set;
        }

        public Type PublicAuthenticationKeyType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the public authentication key.
        /// </summary>
        public byte[] PublicAuthenticationKey
        {
            get;
            set;
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteBytes(AESKey);
            writer.WriteString(PublicAuthenticationKeyType.GetSimplestName());
            writer.WriteBytes(PublicAuthenticationKey);
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            AESKey = reader.ReadBytes();
            PublicAuthenticationKeyType = Type.GetType(reader.ReadString());
            PublicAuthenticationKey = reader.ReadBytes();
        }
    }
}
