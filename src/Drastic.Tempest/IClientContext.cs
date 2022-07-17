// <copyright file="IClientContext.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public interface IClientContext
        : IContext
    {
        /// <summary>
        /// Gets the client connection.
        /// </summary>
        IClientConnection Connection { get; }
    }
}
