// <copyright file="MessageEventArgs.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Holds event data for the <see cref="IConnection.MessageReceived"/> event.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Message}")]
    public class MessageEventArgs
        : ConnectionEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
        /// </summary>
        /// <param name="connection">The connection of the event.</param>
        /// <param name="message">The message received.</param>
        /// <exception cref="ArgumentNullException"><paramref name="connection"/> or <paramref name="message"/> is <c>null</c>.</exception>
        public MessageEventArgs(IConnection connection, Message message)
            : base(connection)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }

            this.Message = message;
        }

        /// <summary>
        /// Gets the message received.
        /// </summary>
        public Message Message
        {
            get;
            private set;
        }
    }
}
