// <copyright file="IContext.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public interface IContext
    {
        /// <summary>
        /// Permanently locks handler registration to improve read performance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This locks handler registration so that no new handlers can registered, improving scalability by
        /// removing locks around message handler storage. This can not be undone.
        /// </para>
        /// </remarks>
        void LockHandlers();

        /// <summary>
        /// Registers a connectionless message handler.
        /// </summary>
        /// <param name="protocol">The protocol of the <paramref name="messageType" />.</param>
        /// <param name="messageType">The message type to register a handler for.</param>
        /// <param name="handler">The handler to register for the message type.</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="protocol"/> is <c>null</c>.</para>
        /// <para>-- or --</para>
        /// <para><paramref name="handler"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">Handler registration is locked by <see cref="LockHandlers"/>.</exception>
        void RegisterConnectionlessMessageHandler(Protocol protocol, ushort messageType, Action<ConnectionlessMessageEventArgs> handler);

        /// <summary>
        /// Registers a message handler.
        /// </summary>
        /// <param name="protocol">The protocol of the <paramref name="messageType" />.</param>
        /// <param name="messageType">The message type to register a handler for.</param>
        /// <param name="handler">The handler to register for the message type.</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="protocol"/> is <c>null</c>.</para>
        /// <para>-- or --</para>
        /// <para><paramref name="handler"/> is <c>null</c>.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">Handler registration is locked by <see cref="LockHandlers"/>.</exception>
        void RegisterMessageHandler(Protocol protocol, ushort messageType, Action<MessageEventArgs> handler);
    }
}
