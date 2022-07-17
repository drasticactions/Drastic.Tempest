// <copyright file="EncryptedMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public class EncryptedMessage
        : ContentMessage
    {
        public EncryptedMessage()
            : base(4)
        {
        }

        public override bool Encrypted
        {
            get { return true; }
        }
    }
}
