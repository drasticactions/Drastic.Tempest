// <copyright file="ISerializer.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Contract for a type that serializes another type.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes <paramref name="element"/> using <paramref name="writer"/>.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="writer">The writer to use to serialize.</param>
        /// <param name="element">The element to serialize.</param>
        void Serialize(ISerializationContext context, IValueWriter writer, object element);

        /// <summary>
        /// Deserializes an element with <paramref name="reader"/>.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="reader">The reader to use to deserialize.</param>
        /// <returns>The deserialized element.</returns>
        object Deserialize(ISerializationContext context, IValueReader reader);
    }
}
