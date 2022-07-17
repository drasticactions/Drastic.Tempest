// <copyright file="ConnectionEventArgs.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Holds event base data for various connection-based events.
    /// </summary>
    public class ConnectionEventArgs
        : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionEventArgs"/> class.
        /// </summary>
        /// <param name="connection">The connection of the event.</param>
        /// <exception cref="ArgumentNullException"><paramref name="connection"/> is <c>null</c>.</exception>
        public ConnectionEventArgs(IConnection connection)
        {
            this.Connection = connection ?? throw new ArgumentNullException("connection");
        }

        /// <summary>
        /// Gets the connection for the event.
        /// </summary>
        public IConnection Connection
        {
            get;
            private set;
        }
    }
}
