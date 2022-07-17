// <copyright file="ClientConnectionEventArgs.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Holds data for client-connection based events.
    /// </summary>
    public class ClientConnectionEventArgs
        : EventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ClientConnectionEventArgs"/> class.
        /// </summary>
        /// <param name="connection">The connection for the event.</param>
        public ClientConnectionEventArgs(IClientConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            Connection = connection;
        }

        /// <summary>
        /// Gets the connection for the event.
        /// </summary>
        public IClientConnection Connection
        {
            get;
            private set;
        }
    }
}
