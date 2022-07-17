// <copyright file="ResettingMemoryStream.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public class ResettingMemoryStream
            : MemoryStream
    {
        public ResettingMemoryStream()
        {
        }

        public ResettingMemoryStream(int capacity)
            : base(capacity)
        {
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.reading)
            {
                this.reading = false;
                Position = 0;
            }

            base.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            if (this.reading)
            {
                this.reading = false;
                Position = 0;
            }

            base.WriteByte(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!this.reading)
            {
                this.reading = true;
                Position = 0;
            }

            return base.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            if (!this.reading)
            {
                this.reading = true;
                Position = 0;
            }

            return base.ReadByte();
        }

        private bool reading;
    }
}
