﻿// <copyright file="MockConnectionProvider.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public class MockConnectionProvider
            : IConnectionProvider
    {
        public MockConnectionProvider(Protocol protocol)
            : this(new[] { protocol })
        {
        }

        public MockConnectionProvider(IEnumerable<Protocol> protocols)
        {
            this.protocols = protocols.ToDictionary(p => p.id);
        }

        public event EventHandler<ConnectionMadeEventArgs> ConnectionMade;

        public IEnumerable<Target> LocalTargets
        {
            get { return Enumerable.Empty<Target>(); }
        }

        public bool IsRunning
        {
            get { return this.running; }
        }

        public void Start(MessageTypes types)
        {
            this.running = true;
        }

        public void Stop()
        {
            this.running = false;
        }

        public IClientConnection GetClientConnection()
        {
            return new MockClientConnection(this)
            {
                ConnectionId = Interlocked.Increment(ref this.cid)
            };
        }

        public IClientConnection GetClientConnection(Protocol protocol)
        {
            return GetClientConnection(new[] { protocol });
        }

        public IClientConnection GetClientConnection(IEnumerable<Protocol> protocols)
        {
            return new MockClientConnection(this, protocols);
        }

        public MockServerConnection GetServerConnection()
        {
            return GetConnections(this.protocols.Values).Item2;
        }

        public MockServerConnection GetServerConnection(Protocol protocol)
        {
            return GetConnections(protocol).Item2;
        }

        public Tuple<MockClientConnection, MockServerConnection> GetConnections(IEnumerable<Protocol> protocols)
        {
            if (protocols == null)
                throw new ArgumentNullException("protocols");

            MockClientConnection c = new MockClientConnection(this, protocols);
            c.ConnectionId = Interlocked.Increment(ref this.cid);
            c.ConnectAsync(new Target(Target.AnyIP, 0), MessageTypes.Reliable);

            return new Tuple<MockClientConnection, MockServerConnection>(c, c.connection);
        }

        public Tuple<MockClientConnection, MockServerConnection> GetConnections(Protocol protocol)
        {
            if (protocol == null)
                throw new ArgumentNullException("protocol");

            return GetConnections(new[] { protocol });
        }

        public void Dispose()
        {
        }

        private int cid;
        private bool running;
        private readonly Dictionary<byte, Protocol> protocols;

        internal void Connect(MockClientConnection connection)
        {
            var c = new MockServerConnection(connection);
            c.ConnectionId = connection.ConnectionId;
            connection.connection = c;

            if (connection.protocols != null)
            {
                foreach (Protocol ip in connection.protocols)
                {
                    Protocol lp;
                    if (!this.protocols.TryGetValue(ip.id, out lp) || !lp.CompatibleWith(ip))
                    {
                        connection.Disconnect(ConnectionResult.IncompatibleVersion);
                        return;
                    }
                }
            }

            var e = new ConnectionMadeEventArgs(c, null);
            OnConnectionMade(e);

            if (e.Rejected)
            {
                connection.Disconnect(ConnectionResult.ConnectionFailed);
                c.Disconnect(ConnectionResult.ConnectionFailed);
            }
        }

        private void OnConnectionMade(ConnectionMadeEventArgs e)
        {
            EventHandler<ConnectionMadeEventArgs> handler = ConnectionMade;
            if (handler != null)
                handler(this, e);
        }
    }
}
