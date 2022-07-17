// <copyright file="TestHelpers.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Text;

namespace Drastic.Tempest.Tests
{
    public static class TestHelpers
    {
        public static string GetLongString(Random r, int length = 1000000)
        {
            if (length <= 0)
                length = 1000000;

            StringBuilder builder = new StringBuilder(length);
            for (int i = 0; i < length; ++i)
                builder.Append((char)r.Next(1, 20));

            return builder.ToString();
        }
    }
}
