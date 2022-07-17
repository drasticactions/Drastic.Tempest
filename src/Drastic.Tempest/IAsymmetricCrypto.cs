// <copyright file="IAsymmetricCrypto.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public interface IAsymmetricCrypto
    {
        byte[] Encrypt(byte[] data);
    }
}
