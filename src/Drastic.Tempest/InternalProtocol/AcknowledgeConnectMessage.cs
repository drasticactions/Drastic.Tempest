// <copyright file="AcknowledgeConnectMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.InternalProtocol
{
    public sealed class AcknowledgeConnectMessage
            : TempestMessage
    {
        public AcknowledgeConnectMessage()
            : base(TempestMessageType.AcknowledgeConnect)
        {
        }

        public override bool Authenticated
        {
            get { return true; }
        }

        public string SignatureHashAlgorithm
        {
            get;
            set;
        }

        public IEnumerable<Protocol> EnabledProtocols
        {
            get;
            set;
        }

        public int ConnectionId
        {
            get;
            set;
        }

        public IAsymmetricKey PublicEncryptionKey
        {
            get;
            set;
        }

        public IAsymmetricKey PublicAuthenticationKey
        {
            get;
            set;
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteString(SignatureHashAlgorithm);

            Protocol[] protocols = EnabledProtocols.ToArray();
            writer.WriteInt32(protocols.Length);
            for (int i = 0; i < protocols.Length; ++i)
                protocols[i].Serialize(context, writer);

            writer.WriteInt32(ConnectionId);

            writer.WriteString(PublicEncryptionKey.GetType().GetSimplestName());
            PublicEncryptionKey.Serialize(context, writer);
            writer.WriteString(PublicAuthenticationKey.GetType().GetSimplestName());
            PublicAuthenticationKey.Serialize(context, writer);
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            SignatureHashAlgorithm = reader.ReadString();

            Protocol[] protocols = new Protocol[reader.ReadInt32()];
            for (int i = 0; i < protocols.Length; ++i)
                protocols[i] = new Protocol(context, reader);

            EnabledProtocols = protocols;

            ConnectionId = reader.ReadInt32();

            PublicEncryptionKey = ReadKey(context, reader);
            PublicAuthenticationKey = ReadKey(context, reader);
        }

        private IAsymmetricKey ReadKey(ISerializationContext context, IValueReader reader)
        {
            string keyType = reader.ReadString();
            Type encryptionKeyType = Type.GetType(keyType, true);
            if (!typeof(IAsymmetricKey).IsAssignableFrom(encryptionKeyType))
                throw new InvalidOperationException("An invalid asymmetric key type was sent.");

            IAsymmetricKey encryptionKey = (IAsymmetricKey)Activator.CreateInstance(encryptionKeyType);
            encryptionKey.Deserialize(context, reader);
            return encryptionKey;
        }
    }
}
