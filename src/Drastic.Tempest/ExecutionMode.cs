// <copyright file="ExecutionMode.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Options for determining how messages are executed.
    /// </summary>
    public enum ExecutionMode
    {
        /// <summary>
        /// Executes all message handlers independently, but in order per connection.
        /// </summary>
        ConnectionOrder,

        /// <summary>
        /// Executes all message handlers in order on a single thread.
        /// </summary>
        GlobalOrder,
    }
}
