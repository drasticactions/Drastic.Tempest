// <copyright file="ISerializationContext.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Contract representing the context for a given serialization.
    /// </summary>
    /// <seealso cref="ISerializer"/>
    public interface ISerializationContext
    {
        /// <summary>
        /// Gets the connection for this serialization.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// Gets the protocols being used in this connection.
        /// </summary>
        /// <remarks>
        /// These protocols represent the agreed upon version of the protocols
        /// by the client and the server. You can use the version of each protocol to
        /// conditionally serialize to support multiple versions of the protocol.
        /// </remarks>
        IReadOnlyDictionary<byte, Protocol> Protocols { get; }
    }
}
