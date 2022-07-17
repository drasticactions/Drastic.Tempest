// <copyright file="ArrayExtensions.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public static class ArrayExtensions
    {
        public static byte[] Copy(this byte[] self, int offset, int length)
        {
            if (self == null)
                throw new ArgumentNullException("self");

            byte[] copy = new byte[length];
            Buffer.BlockCopy(self, offset, copy, 0, length);
            return copy;
        }
    }
}
