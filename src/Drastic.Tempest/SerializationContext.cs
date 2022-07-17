// <copyright file="SerializationContext.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public class SerializationContext
            : ISerializationContext
    {
        public SerializationContext()
        {
        }

        public SerializationContext(IReadOnlyDictionary<byte, Protocol> protocols)
        {
            if (protocols == null)
                throw new ArgumentNullException("protocols");

            Protocols = protocols;
        }

        public SerializationContext(IConnection connection, IReadOnlyDictionary<byte, Protocol> protocols)
            : this(protocols)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            Connection = connection;
        }

        public IConnection Connection
        {
            get;
            private set;
        }

        public IReadOnlyDictionary<byte, Protocol> Protocols
        {
            get;
            private set;
        }

        public SerializationContext WithConnection(IConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");

            return new SerializationContext(connection, Protocols);
        }
    }
}
