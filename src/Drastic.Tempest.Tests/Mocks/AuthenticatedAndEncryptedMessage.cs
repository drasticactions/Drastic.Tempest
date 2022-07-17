// <copyright file="AuthenticatedAndEncryptedMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public class AuthenticatedAndEncryptedMessage
        : ContentMessage
    {
        public AuthenticatedAndEncryptedMessage()
            : base(5)
        {
        }

        public override bool Authenticated
        {
            get { return true; }
        }

        public override bool Encrypted
        {
            get { return true; }
        }
    }
}
