// <copyright file="AuthenticatedMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public class AuthenticatedMessage
        : ContentMessage
    {
        public AuthenticatedMessage()
            : base(3)
        {
        }

        public override bool Authenticated
        {
            get { return true; }
        }
    }
}
