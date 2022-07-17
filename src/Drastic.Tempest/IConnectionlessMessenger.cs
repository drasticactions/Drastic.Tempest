// <copyright file="IConnectionlessMessenger.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public interface IConnectionlessMessenger
        : IListener
    {
        /// <summary>
        /// A connectionless message was received.
        /// </summary>
        event EventHandler<ConnectionlessMessageEventArgs> ConnectionlessMessageReceived;

        /// <summary>
        /// Sends a connectionless <paramref name="message"/> to <paramref name="target"/>.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="target">The target to send the message to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> or <paramref name="target"/> is <c>null</c>.</exception>
        Task SendConnectionlessMessageAsync(Message message, Target target);
    }
}
