// <copyright file="FixedSerializer.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    internal class FixedSerializer
        : ISerializer
    {
        public FixedSerializer(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this.serializer = ObjectSerializer.GetSerializer(type);
        }

        public void Serialize(ISerializationContext context, IValueWriter writer, object element)
        {
            this.serializer.Serialize(context, writer, element);
        }

        public object Deserialize(ISerializationContext context, IValueReader reader)
        {
            return this.serializer.Deserialize(context, reader);
        }

        private readonly ObjectSerializer serializer;
    }
}
