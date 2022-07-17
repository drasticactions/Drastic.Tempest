// <copyright file="MessageHandler.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public class MessageHandler
            : IContext
    {
        public List<Action<MessageEventArgs>> GetHandlers(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            List<Action<MessageEventArgs>> mhandlers = null;

            bool locked = false;
            try
            {
                var mtype = new MessageType(message);
                if (!this.handlersLocked)
                    Monitor.Enter(this.handlers, ref locked);

                if (!this.handlers.TryGetValue(mtype, out mhandlers))
                    this.handlers[mtype] = mhandlers = new List<Action<MessageEventArgs>>();
                else if (locked)
                    mhandlers = mhandlers.ToList();
            }
            finally
            {
                if (locked)
                    Monitor.Exit(this.handlers);
            }

            return mhandlers;
        }

        public List<Action<ConnectionlessMessageEventArgs>> GetConnectionlessHandlers(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            List<Action<ConnectionlessMessageEventArgs>> mhandlers = null;

            bool locked = false;
            try
            {
                var mtype = new MessageType(message);
                if (!this.handlersLocked)
                    Monitor.Enter(this.connectionlessHandlers, ref locked);

                if (!this.connectionlessHandlers.TryGetValue(mtype, out mhandlers))
                    this.connectionlessHandlers[mtype] = mhandlers = new List<Action<ConnectionlessMessageEventArgs>>();
                else if (locked)
                    mhandlers = mhandlers.ToList();
            }
            finally
            {
                if (locked)
                    Monitor.Exit(this.connectionlessHandlers);
            }

            return mhandlers;
        }

        public void LockHandlers()
        {
            this.handlersLocked = true;
        }

        public void RegisterMessageHandler(Protocol protocol, ushort messageType, Action<MessageEventArgs> handler)
        {
            if (protocol == null)
                throw new ArgumentNullException("protocol");
            if (handler == null)
                throw new ArgumentNullException("handler");
            if (this.handlersLocked)
                throw new InvalidOperationException("Handlers locked");

            bool locked = false;

            try
            {
                var mtype = new MessageType(protocol, messageType);
                if (!this.handlersLocked)
                    Monitor.Enter(this.handlers, ref locked);

                List<Action<MessageEventArgs>> mhandlers;
                if (!this.handlers.TryGetValue(mtype, out mhandlers))
                    this.handlers[mtype] = mhandlers = new List<Action<MessageEventArgs>>();

                mhandlers.Add(handler);
            }
            finally
            {
                if (locked)
                    Monitor.Exit(this.handlers);
            }
        }

        public void RegisterConnectionlessMessageHandler(Protocol protocol, ushort messageType, Action<ConnectionlessMessageEventArgs> handler)
        {
            if (protocol == null)
                throw new ArgumentNullException("protocol");
            if (handler == null)
                throw new ArgumentNullException("handler");
            if (this.handlersLocked)
                throw new InvalidOperationException("Handlers locked");

            bool locked = false;

            try
            {
                var mtype = new MessageType(protocol, messageType);
                if (!this.handlersLocked)
                    Monitor.Enter(this.connectionlessHandlers, ref locked);

                List<Action<ConnectionlessMessageEventArgs>> mhandlers;
                if (!this.connectionlessHandlers.TryGetValue(mtype, out mhandlers))
                    this.connectionlessHandlers[mtype] = mhandlers = new List<Action<ConnectionlessMessageEventArgs>>();

                mhandlers.Add(handler);
            }
            finally
            {
                if (locked)
                    Monitor.Exit(this.connectionlessHandlers);
            }
        }

        private bool handlersLocked;
        private readonly Dictionary<MessageType, List<Action<MessageEventArgs>>> handlers = new Dictionary<MessageType, List<Action<MessageEventArgs>>>();
        private readonly Dictionary<MessageType, List<Action<ConnectionlessMessageEventArgs>>> connectionlessHandlers = new Dictionary<MessageType, List<Action<ConnectionlessMessageEventArgs>>>();

        private class MessageType
            : IEquatable<MessageType>
        {
            public MessageType(Protocol protocol, ushort messageType)
            {
                this.protocol = protocol;
                this.messageType = messageType;
            }

            public MessageType(Message msg)
            {
                this.protocol = msg.Protocol;
                this.messageType = msg.MessageType;
            }

            private readonly Protocol protocol;
            private readonly ushort messageType;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != typeof(MessageType))
                    return false;
                return Equals((MessageType)obj);
            }

            public bool Equals(MessageType other)
            {
                if (ReferenceEquals(null, other))
                    return false;
                if (ReferenceEquals(this, other))
                    return true;
                return Equals(other.protocol, this.protocol) && other.messageType == this.messageType;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (this.protocol.GetHashCode() * 397) ^ this.messageType.GetHashCode();
                }
            }

            public static bool operator ==(MessageType left, MessageType right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(MessageType left, MessageType right)
            {
                return !Equals(left, right);
            }
        }
    }
}
