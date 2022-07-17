// <copyright file="DisconnectedEventArgs.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Holds event data for the <see cref="IConnection.Disconnected"/> event.
    /// </summary>
    public class DisconnectedEventArgs
        : ConnectionEventArgs
    {
        /// <summary>
        /// Creates a new instance of <see cref="ConnectionEventArgs"/>.
        /// </summary>
        /// <param name="connection">The connection of the event.</param>
        /// <param name="reason">Result for disconnection.</param>
        /// <exception cref="ArgumentNullException"><paramref name="connection"/> is <c>null</c>.</exception>
        public DisconnectedEventArgs(IConnection connection, ConnectionResult reason)
            : base(connection)
        {
            this.Result = reason;
        }

        public DisconnectedEventArgs(IConnection connection, ConnectionResult reason, string customReason)
            : this(connection, reason)
        {
            CustomReason = customReason;
        }

        public ConnectionResult Result
        {
            get;
            private set;
        }

        public string CustomReason
        {
            get;
            private set;
        }
    }
}
