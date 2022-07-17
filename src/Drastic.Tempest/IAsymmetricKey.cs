// <copyright file="IAsymmetricKey.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public interface IAsymmetricKey
        : ISerializable
    {
        void Serialize(ISerializationContext context, IValueWriter writer, IAsymmetricCrypto crypto);
    }
}
