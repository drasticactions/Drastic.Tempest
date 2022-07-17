// <copyright file="StreamValueWriter.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Text;

namespace Drastic.Tempest
{
    public class StreamValueWriter
            : IValueWriter
    {
        internal readonly Stream stream;

        public StreamValueWriter(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanWrite)
                throw new ArgumentException("Can not write to this stream", "stream");
            if (!BitConverter.IsLittleEndian) // TODO: Support.
                throw new NotSupportedException("Big Endian architecture not supported");

            this.stream = stream;
        }

        public void WriteByte(byte value)
        {
            this.stream.WriteByte(value);
        }

        public void WriteSByte(sbyte value)
        {
            this.stream.WriteByte((byte)value);
        }

        public bool WriteBool(bool value)
        {
            this.stream.WriteByte((byte)((value) ? 1 : 0));

            return value;
        }

        public void WriteBytes(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            WriteInt32(value.Length);
            this.stream.Write(value, 0, value.Length);
        }

        public void WriteBytes(byte[] value, int offset, int length)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            if (offset < 0 || offset >= value.Length)
                throw new ArgumentOutOfRangeException("offset", "offset can not negative or >=data.Length");
            if (length < 0 || offset + length >= value.Length)
                throw new ArgumentOutOfRangeException("length", "length can not be negative or combined with offset longer than the array");

            WriteInt32(length);
            this.stream.Write(value, offset, length);
        }

        public void WriteInt16(short value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void WriteInt32(int value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void WriteInt64(long value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void WriteUInt16(ushort value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void WriteUInt32(uint value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void WriteUInt64(ulong value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void WriteDecimal(decimal value)
        {
            int[] bits = Decimal.GetBits(value);
            WriteInt32(bits.Length);
            for (int i = 0; i < bits.Length; ++i)
                WriteInt32(bits[i]);
        }

        public void WriteSingle(float value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void WriteDouble(double value)
        {
            Write(BitConverter.GetBytes(value));
        }

        public void WriteString(Encoding encoding, string value)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            if (value == null)
            {
                WriteInt32(-1);
                return;
            }

            byte[] data = encoding.GetBytes(value);
            WriteInt32(data.Length);
            Write(data);
        }

        public void Flush()
        {
            this.stream.Flush();
        }

        private void Write(byte[] data)
        {
            this.stream.Write(data, 0, data.Length);
        }
    }
}
