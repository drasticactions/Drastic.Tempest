// <copyright file="MessageEventArgsT.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public class MessageEventArgs<T>
        : ConnectionEventArgs
        where T : Message
    {
        public MessageEventArgs(IConnection connection, T message)
            : base(connection)
        {
            Message = message;
        }

        public T Message
        {
            get;
            private set;
        }
    }
}
