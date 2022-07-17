// <copyright file="TempestMessageType.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.InternalProtocol
{
    /// <summary>
    /// Internal Tempest protocol message type.
    /// </summary>
    public enum TempestMessageType
        : ushort
    {
        /// <summary>
        /// Ping over reliable connections.
        /// </summary>
        Ping = 1,

        /// <summary>
        /// Pong over reliable connections.
        /// </summary>
        Pong = 2,

        /// <summary>
        /// Disconnect with reason.
        /// </summary>
        Disconnect = 3,

        /// <summary>
        /// ClientHello.
        /// </summary>
        Connect = 4,

        /// <summary>
        /// ServerConnected.
        /// </summary>
        Connected = 5,

        /// <summary>
        /// ServerHello.
        /// </summary>
        AcknowledgeConnect = 6,

        /// <summary>
        /// Client finalize connection.
        /// </summary>
        FinalConnect = 7,

        /// <summary>
        /// Acknowledge message received.
        /// </summary>
        Acknowledge = 10,

        /// <summary>
        /// Partial message received.
        /// </summary>
        Partial = 11,
    }
}
