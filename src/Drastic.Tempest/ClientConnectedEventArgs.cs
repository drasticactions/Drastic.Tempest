// <copyright file="ClientConnectedEventArgs.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Holds data for the <see cref="IClientConnection.Connected"/> event.
    /// </summary>
    public class ClientConnectedEventArgs
        : ClientConnectionEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ClientConnectedEventArgs"/> class.
        /// </summary>
        /// <param name="connection">The connection for the event.</param>
        /// <param name="publicKey">The server's public authentication key, if it has one.</param>
        public ClientConnectedEventArgs(IClientConnection connection, IAsymmetricKey publicKey)
            : base(connection)
        {
            ServerPublicKey = publicKey;
        }

        /// <summary>
        /// Gets the server's public authentication key, if encryption or authentication enabled.
        /// </summary>
        public IAsymmetricKey ServerPublicKey
        {
            get;
            private set;
        }
    }
}
