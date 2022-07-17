// <copyright file="ServerMessageSerializer.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using Drastic.Tempest.InternalProtocol;

namespace Drastic.Tempest.Providers.Network
{
    internal class ServerMessageSerializer
            : MessageSerializer
    {
        public ServerMessageSerializer(IAuthenticatedConnection connection, IEnumerable<Protocol> protocols)
            : base(connection, protocols)
        {
        }

        protected override void SignMessage(Message message, string hashAlg, BufferValueWriter writer)
        {
            if (HMAC == null)
                writer.WriteBytes(this.connection.LocalCrypto.HashAndSign(hashAlg, writer.Buffer, 0, writer.Length));
            else
                base.SignMessage(message, hashAlg, writer);
        }

        protected override bool VerifyMessage(string hashAlg, Message message, byte[] signature, byte[] data, int moffset, int length)
        {
            if (HMAC == null)
            {
                var msg = (FinalConnectMessage)message;

                byte[] resized = new byte[length];
                Buffer.BlockCopy(data, moffset, resized, 0, length);

                RSAAsymmetricKey key = (RSAAsymmetricKey)Activator.CreateInstance(msg.PublicAuthenticationKeyType);
                key.Deserialize(new BufferValueReader(msg.PublicAuthenticationKey), this.connection.Encryption);

                this.connection.RemoteKey = key;
                this.connection.RemoteCrypto.ImportKey(key);

                return this.connection.RemoteCrypto.VerifySignedHash(hashAlg, resized, signature);
            }
            else
                return base.VerifyMessage(hashAlg, message, signature, data, moffset, length);
        }
    }
}
