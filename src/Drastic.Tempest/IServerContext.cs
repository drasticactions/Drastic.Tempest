// <copyright file="IServerContext.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Contract for server contexts.
    /// </summary>
    public interface IServerContext
        : IContext
    {
        /// <summary>
        /// Raised when a connection is made.
        /// </summary>
        event EventHandler<ConnectionMadeEventArgs> ConnectionMade;
    }
}
