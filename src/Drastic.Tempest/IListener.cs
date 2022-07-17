// <copyright file="IListener.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Contract for a listener.
    /// </summary>
    /// <see cref="IConnectionProvider"/>
    /// <see cref="IConnectionlessMessenger"/>
    public interface IListener
        : IDisposable
    {
        /// <summary>
        /// Gets whether this listener is currently running or not.
        /// </summary>
        /// <seealso cref="Start"/>
        /// <seealso cref="Stop"/>
        bool IsRunning { get; }

        /// <summary>
        /// Gets the local targets being listened to. Empty until started.
        /// </summary>
        IEnumerable<Target> LocalTargets { get; }

        /// <summary>
        /// Starts the listener.
        /// </summary>
        /// <param name="types">The message types to accept.</param>
        /// <seealso cref="Stop"/>
        void Start(MessageTypes types);

        /// <summary>
        /// Stops the listener.
        /// </summary>
        /// <seealso cref="Start"/>
        void Stop();
    }
}
