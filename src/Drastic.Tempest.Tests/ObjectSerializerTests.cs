// <copyright file="ObjectSerializerTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class ObjectSerializerTests
    {
        private static ISerializationContext context;

        [SetUp]
        public void Setup()
        {
            context = SerializationContextTests.GetContext(MockProtocol.Instance);
        }

        enum TestEnum
            : byte
        {
            Low = 1,
            High = 5
        }

        [Test]
        public void Enum()
        {
            byte[] buffer = new byte[1024];
            var writer = new BufferValueWriter(buffer);
            writer.Write(context, TestEnum.High);
            int len = writer.Length;
            writer.Flush();

            var reader = new BufferValueReader(buffer);

            Assert.AreEqual(TestEnum.High, reader.Read<TestEnum>(context));
            Assert.AreEqual(len, reader.Position);
        }

        [Test]
        public void ISerializable()
        {
            byte[] buffer = new byte[20480];
            var writer = new BufferValueWriter(buffer);

            SerializableTester tester = new SerializableTester
            {
                Name = "MONKEY!",
                Numbers = new[] { 1, 2, 4, 8, 16, 32 }
            };

            var test = new AsyncTest();
            tester.SerializeCalled += test.PassHandler;

            writer.Write(context, tester);
            writer.Flush();

            var reader = new BufferValueReader(buffer);
            var serialized = SerializerExtensions.Read<SerializableTester>(reader, context);

            Assert.IsNotNull(serialized);
            Assert.AreEqual(tester.Name, serialized.Name);
            Assert.IsTrue(tester.Numbers.SequenceEqual(serialized.Numbers), "Numbers does not match");

            test.Assert(1000);
        }

        public class SerializableTester
            : ISerializable
        {
            public SerializableTester()
            {
            }

            protected SerializableTester(IValueReader reader)
            {
                Deserialize(context, reader);
            }

            public event EventHandler SerializeCalled;

            public string Name
            {
                get;
                set;
            }

            public int[] Numbers
            {
                get;
                set;
            }

            public void Serialize(ISerializationContext context, IValueWriter writer)
            {
                OnSerializeCalled(EventArgs.Empty);

                writer.WriteString(Name);

                writer.WriteInt32(Numbers.Length);
                for (int i = 0; i < Numbers.Length; ++i)
                    writer.WriteInt32(Numbers[i]);
            }

            public void Deserialize(ISerializationContext context, IValueReader reader)
            {
                Name = reader.ReadString();

                Numbers = new int[reader.ReadInt32()];
                for (int i = 0; i < Numbers.Length; ++i)
                    Numbers[i] = reader.ReadInt32();
            }

            private void OnSerializeCalled(EventArgs e)
            {
                var called = SerializeCalled;
                if (called != null)
                    called(this, e);
            }
        }
    }
}
