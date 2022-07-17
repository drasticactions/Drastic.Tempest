// <copyright file="MessageTypes.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    [Flags]
    public enum MessageTypes
    {
        /// <summary>
        /// Reliable in-order delivery.
        /// </summary>
        Reliable = 1,

        /// <summary>
        /// Unreliable no-order-guarantee delivery.
        /// </summary>
        Unreliable = 2,

        /// <summary>
        /// All message delivery types.
        /// </summary>
        All = Reliable | Unreliable,
    }
}
