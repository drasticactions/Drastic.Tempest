// <copyright file="AsyncTestT.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public class AsyncTest<T>
            : AsyncTest
            where T : EventArgs
    {
        public AsyncTest(Action<T> passTest)
            : base(e => passTest((T)e))
        {
        }

        public AsyncTest(Action<T> passTest, bool multiple)
            : base(e => passTest((T)e), multiple)
        {
        }

        public AsyncTest(Action<T> passTest, int times)
            : base(e => passTest((T)e), times)
        {
        }

        public AsyncTest(Func<T, bool> passPredicate)
            : base(e => passPredicate((T)e))
        {
        }

        public AsyncTest(Func<T, bool> passPredicate, bool multiple)
            : base(e => passPredicate((T)e), multiple)
        {
        }

        public AsyncTest(Func<T, bool> passPredicate, int times)
            : base(e => passPredicate((T)e), times)
        {
        }
    }
}
