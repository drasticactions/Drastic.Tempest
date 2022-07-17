// <copyright file="ConnectionResult.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Reasons for disconnection.
    /// </summary>
    public enum ConnectionResult
        : byte
    {
        /// <summary>
        /// Connection lost or killed for an unknown reason.
        /// </summary>
        FailedUnknown = 0,

        /// <summary>
        /// Connection succeeded.
        /// </summary>
        Success = 1,

        /// <summary>
        /// The connection failed to connect to begin with.
        /// </summary>
        ConnectionFailed = 2,

        /// <summary>
        /// The server does not support the client's version of the protocol.
        /// </summary>
        IncompatibleVersion = 3,

        /// <summary>
        /// The client failed during the handshake.
        /// </summary>
        FailedHandshake = 4,

        /// <summary>
        /// A signed message failed verification.
        /// </summary>
        MessageAuthenticationFailed = 5,

        /// <summary>
        /// An encrypted message failed decryption.
        /// </summary>
        EncryptionMismatch = 6,

        /// <summary>
        /// An application specified result.
        /// </summary>
        Custom = 7,

        /// <summary>
        /// The connection timed out.
        /// </summary>
        TimedOut = 8,
    }
}
