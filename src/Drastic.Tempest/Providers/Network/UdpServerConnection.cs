﻿// <copyright file="UdpServerConnection.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Net;
using System.Security.Cryptography;
using Drastic.Tempest.InternalProtocol;

namespace Drastic.Tempest.Providers.Network
{
    public sealed class UdpServerConnection
            : UdpConnection, IServerConnection, IAuthenticatedConnection
    {
        internal UdpServerConnection(int connectionId, EndPoint remoteEndpoint, UdpConnectionProvider provider)
            : base(provider.protocols.Values)
        {
            ConnectionId = connectionId;
            IPEndPoint = (IPEndPoint)remoteEndpoint;
            RemoteTarget = remoteEndpoint.ToTarget();
            this.provider = provider;

            this.socket = this.provider.GetSocket(remoteEndpoint);
        }

        internal UdpServerConnection(int connectionId, EndPoint remoteEndpoint, UdpConnectionProvider provider, RSACrypto remoteCrypto, RSACrypto localCrypto, RSAAsymmetricKey key)
            : base(provider.protocols.Values)
        {
            this.remoteCrypto = remoteCrypto;
            this.localCrypto = localCrypto;
            LocalKey = key;

            ConnectionId = connectionId;
            IPEndPoint = (IPEndPoint)remoteEndpoint;
            RemoteTarget = remoteEndpoint.ToTarget();
            this.provider = provider;

            this.socket = this.provider.GetSocket(remoteEndpoint);
        }

        private readonly UdpConnectionProvider provider;
        private bool receivedProtocols;

        protected override void Cleanup()
        {
            base.Cleanup();

            this.provider.Disconnect(this);
        }

        protected override bool IsConnecting
        {
            get { return !this.formallyConnected; }
        }

        protected override void OnTempestMessage(MessageEventArgs e)
        {
            switch (e.Message.MessageType)
            {
                case (ushort)TempestMessageType.Connect:
                    OnConnectMessage((ConnectMessage)e.Message);
                    break;

                case (ushort)TempestMessageType.FinalConnect:
                    OnFinalConnectMessage((FinalConnectMessage)e.Message);
                    break;

                default:
                    base.OnTempestMessage(e);
                    break;
            }
        }

        private void OnFinalConnectMessage(FinalConnectMessage msg)
        {
            if (!this.receivedProtocols)
            {
                Disconnect(ConnectionResult.FailedHandshake);
                return;
            }

            if (msg.AESKey == null || msg.AESKey.Length == 0 || msg.PublicAuthenticationKey == null)
            {
                Disconnect(ConnectionResult.FailedHandshake);
                return;
            }

            try
            {
                byte[] aeskey = this.provider.pkEncryption.Decrypt(msg.AESKey);

                this.serializer.HMAC = new HMACSHA256(aeskey);
                this.serializer.AES = new AesManaged { KeySize = 256, Key = aeskey };
            }
            catch
            {
                Disconnect(ConnectionResult.FailedHandshake);
                return;
            }

            this.formallyConnected = true;
            this.provider.Connect(this);

            SendAsync(new ConnectedMessage { ConnectionId = ConnectionId });
        }

        private void OnConnectMessage(ConnectMessage msg)
        {
            if (!msg.Protocols.Any())
            {
                DisconnectAsync(ConnectionResult.FailedHandshake);
                return;
            }

            string signingHashAlgorithm = null;
            if (this.requiresHandshake)
            {
                bool foundHashAlg = false;
                if (this.localCrypto != null)
                {
                    foreach (string hashAlg in this.localCrypto.SupportedHashAlgs)
                    {
                        if (msg.SignatureHashAlgorithms.Contains(hashAlg))
                        {
                            signingHashAlgorithm = hashAlg;
                            foundHashAlg = true;
                            break;
                        }
                    }
                }

                if (!foundHashAlg)
                {
                    this.provider.SendConnectionlessMessageAsync(new DisconnectMessage { Reason = ConnectionResult.IncompatibleVersion }, RemoteTarget);
                    Disconnect(ConnectionResult.FailedHandshake);
                    return;
                }
            }

            foreach (Protocol protocol in msg.Protocols)
            {
                Protocol lp;
                if (!this.provider.protocols.TryGetValue(protocol.id, out lp) || !lp.CompatibleWith(protocol))
                {
                    this.provider.SendConnectionlessMessageAsync(new DisconnectMessage { Reason = ConnectionResult.IncompatibleVersion }, RemoteTarget);
                    Disconnect(ConnectionResult.IncompatibleVersion);
                    return;
                }
            }

            var protocols = this.provider.protocols.Values.Intersect(msg.Protocols);
            this.serializer = new ServerMessageSerializer(this, protocols);
            this.serializer.SigningHashAlgorithm = signingHashAlgorithm;
            this.receivedProtocols = true;

            if (!this.requiresHandshake)
            {
                this.formallyConnected = true;
                this.provider.Connect(this);

                SendAsync(new ConnectedMessage { ConnectionId = ConnectionId });
            }
            else
            {
                SendAsync(new AcknowledgeConnectMessage
                {
                    SignatureHashAlgorithm = this.serializer.SigningHashAlgorithm,
                    EnabledProtocols = Protocols,
                    ConnectionId = ConnectionId,
                    PublicAuthenticationKey = this.provider.PublicKey,
                    PublicEncryptionKey = this.provider.PublicKey
                });
            }
        }

        RSACrypto IAuthenticatedConnection.LocalCrypto
        {
            get { return this.localCrypto; }
        }

        RSACrypto IAuthenticatedConnection.Encryption
        {
            get { return this.provider.pkEncryption; }
        }

        IAsymmetricKey IAuthenticatedConnection.LocalKey
        {
            get { return LocalKey; }
            set { LocalKey = value; }
        }

        RSACrypto IAuthenticatedConnection.RemoteCrypto
        {
            get { return this.remoteCrypto; }
        }

        IAsymmetricKey IAuthenticatedConnection.RemoteKey
        {
            get { return RemoteKey; }
            set { RemoteKey = value; }
        }
    }
}
