// <copyright file="IAuthenticatedConnection.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Providers.Network
{
    // HACK
    internal interface IAuthenticatedConnection
        : IConnection
    {
        RSACrypto LocalCrypto { get; }
        new IAsymmetricKey LocalKey { get; set; }

        RSACrypto RemoteCrypto { get; }
        new IAsymmetricKey RemoteKey { get; set; }

        RSACrypto Encryption { get; }
    }
}
