// <copyright file="SerializerT.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Reflection;

namespace Drastic.Tempest
{
    public static class Serializer<T>
    {
        public static readonly ISerializer<T> Default = new DefaultSerializer();

        private class DefaultSerializer
            : ISerializer<T>
        {
            public void Serialize(ISerializationContext context, IValueWriter writer, T element)
            {
                Type etype;
                if (element != null)
                {
                    etype = element.GetType();
                    if (etype.GetTypeInfo().IsValueType && typeof(T) == typeof(object))
                        etype = typeof(object);
                }
                else
                    etype = typeof(object);

                ObjectSerializer.GetSerializer(etype).Serialize(context, writer, element);
            }

            public T Deserialize(ISerializationContext context, IValueReader reader)
            {
                return (T)ObjectSerializer.GetSerializer(typeof(T)).Deserialize(context, reader);
            }
        }
    }
}
