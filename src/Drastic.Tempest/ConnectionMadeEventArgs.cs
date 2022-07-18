// <copyright file="ConnectionMadeEventArgs.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Provides data for the <see cref="IConnectionProvider.ConnectionMade"/> event.
    /// </summary>
    public class ConnectionMadeEventArgs
        : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMadeEventArgs"/> class.
        /// </summary>
        /// <param name="connection">The newly made connection.</param>
        /// <param name="publicKey">The clients public authentication key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="connection"/> is <c>null</c>.</exception>
        public ConnectionMadeEventArgs(IServerConnection connection, IAsymmetricKey publicKey)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            Connection = connection;
            ClientPublicKey = publicKey;
        }

        /// <summary>
        /// Gets the clients public authentication key, if present.
        /// </summary>
        public IAsymmetricKey ClientPublicKey
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the newly formed connection.
        /// </summary>
        public IServerConnection Connection
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets whether to reject this connection.
        /// </summary>
        public bool Rejected
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"Rejected: {Rejected}, From: {Connection.ConnectionId}, IsConnected: {Connection.IsConnected}";
        }
    }
}
