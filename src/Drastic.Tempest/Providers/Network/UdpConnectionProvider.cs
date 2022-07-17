// <copyright file="UdpConnectionProvider.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Drastic.Tempest.InternalProtocol;

namespace Drastic.Tempest.Providers.Network
{
    public sealed class UdpConnectionProvider
            : UdpConnectionlessListener, IConnectionProvider
    {
        public UdpConnectionProvider(int port, Protocol protocol)
            : base(new[] { protocol }, port)
        {
            if (protocol == null)
                throw new ArgumentNullException("protocol");
            if (port <= 0)
                throw new ArgumentOutOfRangeException("port");

            ValidateProtocols(this.protocols.Values);
        }

        public UdpConnectionProvider(int port, Protocol protocol, RSAAsymmetricKey authKey)
            : base(new[] { protocol }, port)
        {
            if (protocol == null)
                throw new ArgumentNullException("protocol");
            if (authKey == null)
                throw new ArgumentNullException("authKey");
            if (port <= 0)
                throw new ArgumentOutOfRangeException("port");

            this.crypto = new RSACrypto();
            this.crypto.ImportKey(authKey);

            this.pkEncryption = new RSACrypto();
            this.pkEncryption.ImportKey(authKey);

            this.authKey = authKey;

            ValidateProtocols(this.protocols.Values);
        }

        public UdpConnectionProvider(int port, IEnumerable<Protocol> protocols, RSAAsymmetricKey authKey)
            : base(protocols, port)
        {
            if (protocols == null)
                throw new ArgumentNullException("protocols");
            if (authKey == null)
                throw new ArgumentNullException("authKey");
            if (port <= 0)
                throw new ArgumentOutOfRangeException("port");

            this.crypto = new RSACrypto();
            this.crypto.ImportKey(authKey);
            this.authKey = authKey;

            ValidateProtocols(this.protocols.Values);
        }

        public event EventHandler<ConnectionMadeEventArgs> ConnectionMade;

        public RSAAsymmetricKey PublicKey
        {
            get { return this.authKey; }
        }

        public override void Start(MessageTypes types)
        {
            base.Start(types);

            Timer timer = new Timer(100);
            timer.TimesUp += OnDeliveryTimer;
            timer.Start();
            this.deliveryTimer = timer;
        }

        public override void Stop()
        {
            Timer timer = this.deliveryTimer;
            if (timer != null)
            {
                timer.TimesUp -= OnDeliveryTimer;
                timer.Dispose();
            }

            base.Stop();
        }

        internal RSACrypto pkEncryption;
        private RSACrypto crypto;
        private RSAAsymmetricKey authKey;

        private int nextConnectionId;
        private readonly ConcurrentDictionary<int, UdpServerConnection> connections = new ConcurrentDictionary<int, UdpServerConnection>();

        private Timer deliveryTimer;

        private readonly long pingTimeout = Stopwatch.Frequency * 15;

        protected override bool TryGetConnection(int connectionId, out UdpConnection connection)
        {
            connection = null;

            UdpServerConnection server;
            bool found = this.connections.TryGetValue(connectionId, out server);
            if (found)
            {
                connection = server;
                return true;
            }

            return false;
        }

        internal void Connect(UdpServerConnection connection)
        {
            var args = new ConnectionMadeEventArgs(connection, connection.RemoteKey);
            OnConnectionMade(args);

            if (args.Rejected)
                connection.Dispose();
        }

        internal void Disconnect(UdpServerConnection connection)
        {
            UdpServerConnection o;
            this.connections.TryRemove(connection.ConnectionId, out o);
        }

        private void OnDeliveryTimer(object sender, EventArgs eventArgs)
        {
            long now = Stopwatch.GetTimestamp();

            foreach (UdpServerConnection connection in this.connections.Values)
            {
                if (!connection.IsConnected)
                    continue;

                connection.ResendPending();
                connection.CheckPendingTimeouts();

                if ((now - connection.lastReceiveActivity) > this.pingTimeout)
                    connection.DisconnectAsync(ConnectionResult.TimedOut);
            }
        }

        private void OnConnectionMade(ConnectionMadeEventArgs e)
        {
            var handler = this.ConnectionMade;
            if (handler != null)
                handler(this, e);
        }

        internal Socket GetSocket(EndPoint endPoint)
        {
            if (endPoint.AddressFamily == AddressFamily.InterNetwork)
                return this.socket4;
            else if (endPoint.AddressFamily == AddressFamily.InterNetworkV6)
                return this.socket6;
            else
                throw new ArgumentException();
        }

        protected override void OnConnectionlessTempestMessage(TempestMessage tempestMessage, Target target)
        {
            ConnectMessage connect = tempestMessage as ConnectMessage;
            if (connect != null)
            {
                UdpServerConnection connection;
                if (this.authKey != null)
                    connection = new UdpServerConnection(GetConnectionId(), target.ToEndPoint(), this, new RSACrypto(), this.crypto, this.authKey);
                else
                    connection = new UdpServerConnection(GetConnectionId(), target.ToEndPoint(), this);

                if (!this.connections.TryAdd(connection.ConnectionId, connection))
                    throw new InvalidOperationException("Reused connection ID");

                connection.Receive(connect);
            }
            else
                base.OnConnectionlessTempestMessage(tempestMessage, target);
        }

        private int GetConnectionId()
        {
            return Interlocked.Increment(ref nextConnectionId);
        }

        private void ValidateProtocols(IEnumerable<Protocol> newProtocols)
        {
            foreach (Protocol newProtocol in newProtocols)
            {
                if (newProtocol == null)
                    throw new ArgumentException("null Protocol in protocols", "protocols");

                if (newProtocol.id != 1 && newProtocol.RequiresHandshake && this.crypto == null)
                    throw new ArgumentException("Protocol requires handshake, but no crypto provided", "protocols");
            }
        }
    }
}
