// <copyright file="ISerializable.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Contract representing a type that can serialize and deserialize itself.
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Serializes the instance to the <paramref name="writer"/>.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="writer">The <see cref="IValueWriter"/> to serialize with.</param>
        void Serialize(ISerializationContext context, IValueWriter writer);

        /// <summary>
        /// Deserializes the instance from the <paramref name="reader"/>.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="reader">The <see cref="IValueReader"/> to deserialize with.</param>
        void Deserialize(ISerializationContext context, IValueReader reader);
    }
}
