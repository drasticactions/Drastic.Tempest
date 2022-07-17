// <copyright file="HeaderState.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public enum HeaderState
    {
        /// <summary>
        /// Default state.
        /// </summary>
        Empty = 0,

        /// <summary>
        /// The protocol identifier.
        /// </summary>
        Protocol = 1,

        /// <summary>
        /// The connection ID.
        /// </summary>
        CID = 2,

        /// <summary>
        /// The message type.
        /// </summary>
        Type = 3,

        /// <summary>
        /// The message length.
        /// </summary>
        Length = 4,

        /// <summary>
        /// The encryption initialization vector.
        /// </summary>
        IV = 5,

        /// <summary>
        /// The message ID.
        /// </summary>
        MessageId = 6,

        /// <summary>
        /// The message ID of a message being responded to.
        /// </summary>
        ResponseMessageId = 7,

        /// <summary>
        /// The header has been completely read.
        /// </summary>
        Complete = 8,
    }
}
