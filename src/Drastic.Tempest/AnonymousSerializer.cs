// <copyright file="AnonymousSerializer.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public class AnonymousSerializer<T>
        : ISerializer<T>
    {
        public AnonymousSerializer(Action<ISerializationContext, IValueWriter, T> serializer, Func<ISerializationContext, IValueReader, T> deserialzier)
        {
            if (serializer == null)
                throw new ArgumentNullException("serializer");
            if (deserialzier == null)
                throw new ArgumentNullException("deserialzier");

            this.serializer = serializer;
            this.deserialzier = deserialzier;
        }

        public void Serialize(ISerializationContext context, IValueWriter writer, T element)
        {
            this.serializer(context, writer, element);
        }

        public T Deserialize(ISerializationContext context, IValueReader reader)
        {
            return this.deserialzier(context, reader);
        }

        private readonly Action<ISerializationContext, IValueWriter, T> serializer;
        private readonly Func<ISerializationContext, IValueReader, T> deserialzier;
    }
}
