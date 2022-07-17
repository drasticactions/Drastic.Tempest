// <copyright file="ClientMessageSerializer.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using Drastic.Tempest.InternalProtocol;

namespace Drastic.Tempest.Providers.Network
{
    internal class ClientMessageSerializer
        : MessageSerializer
    {
        public ClientMessageSerializer(IAuthenticatedConnection connection, IEnumerable<Protocol> protocols)
            : base(connection, protocols)
        {
        }

        public ClientMessageSerializer(ClientMessageSerializer serializer)
            : base(serializer)
        {
        }

        protected override void SignMessage(Message message, string hashAlg, BufferValueWriter writer)
        {
            if (message is FinalConnectMessage)
                writer.WriteBytes(this.connection.LocalCrypto.HashAndSign(hashAlg, writer.Buffer, 0, writer.Length));
            else
                base.SignMessage(message, hashAlg, writer);
        }

        protected override bool VerifyMessage(string hashAlg, Message message, byte[] signature, byte[] data, int moffset, int length)
        {
            if (HMAC == null)
            {
                byte[] resized = new byte[length];
                Buffer.BlockCopy(data, moffset, resized, 0, length);

                var msg = (AcknowledgeConnectMessage)message;

                this.connection.RemoteKey = msg.PublicAuthenticationKey;
                this.connection.RemoteCrypto.ImportKey((RSAAsymmetricKey)this.connection.RemoteKey);

                SigningHashAlgorithm = msg.SignatureHashAlgorithm;
                return this.connection.RemoteCrypto.VerifySignedHash(SigningHashAlgorithm, resized, signature);
            }
            else
                return base.VerifyMessage(hashAlg, message, signature, data, moffset, length);
        }
    }
}
