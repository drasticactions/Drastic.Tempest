// <copyright file="ISerializerT.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Contract for a type that serializes another type.
    /// </summary>
    /// <typeparam name="T">The type to serialize and deserialize.</typeparam>
    public interface ISerializer<T>
    {
        /// <summary>
        /// Serializes <paramref name="element"/> using <paramref name="writer"/>.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="writer">The writer to use to serialize.</param>
        /// <param name="element">The element to serialize.</param>
        void Serialize(ISerializationContext context, IValueWriter writer, T element);

        /// <summary>
        /// Deserializes an element with <paramref name="reader"/>.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="reader">The reader to use to deserialize.</param>
        /// <returns>The deserialized element.</returns>
        T Deserialize(ISerializationContext context, IValueReader reader);
    }
}
