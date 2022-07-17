// <copyright file="IClientConnection.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public interface IClientConnection
        : IConnection
    {
        /// <summary>
        /// Raised when the connection has connected.
        /// </summary>
        event EventHandler<ClientConnectionEventArgs> Connected;

        /// <summary>
        /// Attempts to asynchronously connect to the <paramref name="target"/> for <paramref name="messageTypes"/>.
        /// </summary>
        /// <param name="target">The target to connect to.</param>
        /// <param name="messageTypes">The type of messages to connect for.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <c>null</c>.</exception>
        Task<ClientConnectionResult> ConnectAsync(Target target, MessageTypes messageTypes);
    }

}
