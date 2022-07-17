// <copyright file="MessageFactory.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Reflection;

namespace Drastic.Tempest
{
    public class MessageFactory
    {
        internal MessageFactory()
        {
        }

        public bool RequiresHandshake
        {
            get;
            private set;
        }

        /// <summary>
        /// Registers types with a method of construction.
        /// </summary>
        /// <param name="messageTypes">The types to register.</param>
        /// <exception cref="ArgumentNullException"><paramref name="messageTypes"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="messageTypes"/> contains non-implementations of <see cref="Message"/>
        /// or <paramref name="messageTypes"/> contains duplicate <see cref="Message.MessageType"/>s.</exception>
        public void Register(IEnumerable<KeyValuePair<Type, Func<Message>>> messageTypes)
        {
            RegisterTypesWithCtors(messageTypes, false);
        }

        /// <summary>
        /// Creates a new instance of the <paramref name="messageType"/>.
        /// </summary>
        /// <param name="messageType">The unique message identifier in the protocol for the desired message.</param>
        /// <returns>A new instance of the <paramref name="messageType"/>, or <c>null</c> if this type has not been registered.</returns>
        public Message Create(ushort messageType)
        {
            Func<Message> mCtor;
            if (!this.messageCtors.TryGetValue(messageType, out mCtor))
                return null;

            return mCtor();
        }

        private readonly ConcurrentDictionary<ushort, Func<Message>> messageCtors = new ConcurrentDictionary<ushort, Func<Message>>();

        private void RegisterTypesWithCtors(IEnumerable<KeyValuePair<Type, Func<Message>>> messageTypes, bool ignoreDupes)
        {
            if (messageTypes == null)
                throw new ArgumentNullException("messageTypes");

            TypeInfo mtype = typeof(Message).GetTypeInfo();

            foreach (var kvp in messageTypes)
            {
                if (!mtype.IsAssignableFrom(kvp.Key.GetTypeInfo()))
                    throw new ArgumentException(String.Format("{0} is not an implementation of Message", kvp.Key.Name), "messageTypes");
                if (kvp.Key.GetTypeInfo().IsGenericType || kvp.Key.GetTypeInfo().IsGenericTypeDefinition)
                    throw new ArgumentException(String.Format("{0} is a generic type which is unsupported", kvp.Key.Name), "messageTypes");

                Message m = kvp.Value();
                if (m.Protocol != (Protocol)this)
                    continue;

                if (m.Authenticated || m.Encrypted)
                    RequiresHandshake = true;

                if (!this.messageCtors.TryAdd(m.MessageType, kvp.Value))
                {
                    if (ignoreDupes)
                        continue;

                    throw new ArgumentException(String.Format("A message of type {0} has already been registered.", m.MessageType), "messageTypes");
                }
            }
        }

        private static readonly Type[] EmptyTypes = new Type[0];
    }
}
