// <copyright file="ContextExtensions.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public static class ContextExtensions
    {
        /// <summary>
        /// Registers a message handler.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <param name="self">The context to register the message handler to.</param>
        /// <param name="handler">The message handler to register.</param>
        public static void RegisterMessageHandler<T>(this IContext self, Action<MessageEventArgs<T>> handler)
            where T : Message, new()
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (handler == null)
                throw new ArgumentNullException("handler");

            T msg = new T();
            self.RegisterMessageHandler(msg.Protocol, msg.MessageType, e => handler(new MessageEventArgs<T>(e.Connection, (T)e.Message)));
        }
    }
}
