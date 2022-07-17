// <copyright file="IConnectionProvider.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Contract for a provider of connections.
    /// </summary>
    public interface IConnectionProvider
        : IListener
    {
        /// <summary>
        /// A new connection was made.
        /// </summary>
        event EventHandler<ConnectionMadeEventArgs> ConnectionMade;
    }
}
