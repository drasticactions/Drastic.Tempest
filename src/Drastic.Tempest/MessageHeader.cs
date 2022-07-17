// <copyright file="MessageHeader.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public class MessageHeader
    {
        public HeaderState State
        {
            get;
            set;
        }

        public Protocol Protocol
        {
            get;
            set;
        }

        public Message Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the total length of the message (including <see cref="HeaderLength"/>).
        /// </summary>
        public int MessageLength
        {
            get;
            set;
        }

        public int HeaderLength
        {
            get;
            set;
        }

        public ISerializationContext SerializationContext
        {
            get;
            set;
        }

        public byte[] IV
        {
            get;
            set;
        }

        public bool IsStillEncrypted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets whether the message is a response to another message or not.
        /// </summary>
        public bool IsResponse
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the message being responded to.
        /// </summary>
        public int ResponseMessageId
        {
            get;
            set;
        }

        public int MessageId
        {
            get;
            set;
        }

        public int ConnectionId
        {
            get;
            set;
        }
    }
}
