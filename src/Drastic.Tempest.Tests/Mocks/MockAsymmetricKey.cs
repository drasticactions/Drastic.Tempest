// <copyright file="MockAsymmetricKey.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public class MockAsymmetricKey
            : RSAAsymmetricKey
    {
        public void Serialize(ISerializationContext context, IValueWriter writer)
        {
        }

        public void Deserialize(ISerializationContext context, IValueReader reader)
        {
        }

        public byte[] PublicSignature
        {
            get { return new byte[0]; }
        }

        public void Serialize(IValueWriter writer, RSACrypto crypto, bool includePrivate)
        {
        }

        public void Deserialize(IValueReader reader, RSACrypto crypto)
        {
        }
    }
}
