// <copyright file="ConnectionlessMessageEventArgs.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Provides data for the <see cref="IConnectionlessMessenger.ConnectionlessMessageReceived"/> event.
    /// </summary>
    public class ConnectionlessMessageEventArgs
        : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionlessMessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message that was received connectionlessly.</param>
        /// <param name="from">The <see cref="Target"/> the message came from.</param>
        /// <param name="messenger">The <see cref="IConnectionlessMessenger"/> the message was received from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> or <paramref name="from"/> is <c>null</c>.</exception>
        public ConnectionlessMessageEventArgs(Message message, Target from, IConnectionlessMessenger messenger)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (from == null)
                throw new ArgumentNullException("from");
            if (messenger == null)
                throw new ArgumentNullException("messenger");

            Message = message;
            From = from;
            Messenger = messenger;
        }

        /// <summary>
        /// Gets the received message.
        /// </summary>
        public Message Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Where the message came from.
        /// </summary>
        public Target From
        {
            get;
            private set;
        }

        public IConnectionlessMessenger Messenger
        {
            get;
            private set;
        }
    }
}
