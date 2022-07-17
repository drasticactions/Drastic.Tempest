// <copyright file="Message.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    [System.Diagnostics.DebuggerDisplay("{Header.MessageId}:{GetType()}")]
    public abstract class Message
    {
        protected Message(Protocol protocol, ushort messageType)
        {
            if (ReferenceEquals(protocol, null))
                throw new ArgumentNullException("protocol");

            Protocol = protocol;
            MessageType = messageType;
        }

        /// <summary>
        /// Gets whether this message must be reliably delivered or not.
        /// </summary>
        public virtual bool MustBeReliable
        {
            get { return true; }
        }

        /// <summary>
        /// Gets whether this message prefers reliable delivery if possible.
        /// </summary>
        public virtual bool PreferReliable
        {
            get { return true; }
        }

        /// <summary>
        /// Gets whether this message can be received without forming a connection.
        /// </summary>
        public virtual bool AcceptedConnectionlessly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether this message is hashed and signed.
        /// </summary>
        public virtual bool Authenticated
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether this message is encrypted.
        /// </summary>
        public virtual bool Encrypted
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the protocol this message belongs to.
        /// </summary>
        public Protocol Protocol
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the unique identifier for this message within its protocol.
        /// </summary>
        public ushort MessageType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets information about the message for provider implementations.
        /// </summary>
        public MessageHeader Header
        {
            get;
            set;
        }

        /// <summary>
        /// Writes the message payload with <paramref name="writer"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="writer">The writer to use for writing the payload.</param>
        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is <c>null</c>.</exception>
        public abstract void WritePayload(ISerializationContext context, IValueWriter writer);

        /// <summary>
        /// Reads the message payload with <paramref name="reader"/>.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reader">The reader to use for reading the payload.</param>
        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is <c>null</c>.</exception>
        public abstract void ReadPayload(ISerializationContext context, IValueReader reader);
    }
}
