// <copyright file="ClientConnectionResult.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Holds data for <see cref="IClientConnection.ConnectAsync"/> results.
    /// </summary>
    public class ClientConnectionResult
    {
        /// <summary>
        /// Constructs and initializes a new instance of the <see cref="ClientConnectionResult"/> class.
        /// </summary>
        /// <param name="result">The result of the connection attempt.</param>
        /// <param name="publicKey">The server's public authentication key, if it has one.</param>
        public ClientConnectionResult(ConnectionResult result, IAsymmetricKey publicKey)
        {
            if (!Enum.IsDefined(typeof(ConnectionResult), result))
                throw new ArgumentException("result is not a valid member of ConnectionResult", "result");

            Result = result;
            ServerPublicKey = publicKey;
        }

        /// <summary>
        /// Gets the connection result.
        /// </summary>
        public ConnectionResult Result
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the server's public authentication key, if encryption or authentication enabled.
        /// </summary>
        public IAsymmetricKey ServerPublicKey
        {
            get;
            private set;
        }
    }
}
