// <copyright file="CollectionExtensions.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    internal static class CollectionExtensions
    {
        public static bool SequenceEqual<T>(this T[] self, T[] sequence)
        {
            if (self == null)
                throw new ArgumentNullException("self");
            if (sequence == null)
                throw new ArgumentNullException("sequence");

            if (self.Length != sequence.Length)
                return false;

            bool equal = true;
            for (int i = 0; i < self.Length; ++i)
            {
                if (!self[i].Equals(sequence[i]))
                {
                    equal = false;
                    break;
                }
            }

            return equal;
        }
    }
}
