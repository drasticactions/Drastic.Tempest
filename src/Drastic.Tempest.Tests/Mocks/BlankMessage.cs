// <copyright file="BlankMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Tempest.Tests
{
    public class BlankMessage
        : Message
    {
        public BlankMessage()
            : base(MockProtocol.Instance, 2)
        {
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
        }
    }
}
