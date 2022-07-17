// <copyright file="StreamValueReader.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Text;

namespace Drastic.Tempest
{
    public class StreamValueReader
            : IValueReader
    {
        private readonly Stream stream;

        public StreamValueReader(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new ArgumentException("Can not read from this stream", "stream");

            this.stream = stream;
        }

        public bool ReadBool()
        {
            return (ReadByte() == 1);
        }

        public byte[] ReadBytes()
        {
            int count = ReadInt32();
            return ReadBytes(count);
        }

        public byte[] ReadBytes(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "count must be >= 0");

            byte[] buffer = new byte[count];

            int i = 0;
            int bytes;
            while (i < buffer.Length && (bytes = this.stream.Read(buffer, i, count)) > 0)
            {
                i += bytes;
                count -= bytes;
            }

            return buffer;
        }

        public sbyte ReadSByte()
        {
            return (sbyte)this.stream.ReadByte();
        }

        public short ReadInt16()
        {
            return BitConverter.ToInt16(ReadBytes(sizeof(short)), 0);
        }

        public int ReadInt32()
        {
            return BitConverter.ToInt32(ReadBytes(sizeof(int)), 0);
        }

        public long ReadInt64()
        {
            return BitConverter.ToInt64(ReadBytes(sizeof(long)), 0);
        }

        public byte ReadByte()
        {
            return (byte)this.stream.ReadByte();
        }

        public ushort ReadUInt16()
        {
            return BitConverter.ToUInt16(ReadBytes(sizeof(ushort)), 0);
        }

        public uint ReadUInt32()
        {
            return BitConverter.ToUInt32(ReadBytes(sizeof(uint)), 0);
        }

        public ulong ReadUInt64()
        {
            return BitConverter.ToUInt64(ReadBytes(sizeof(ulong)), 0);
        }

        public string ReadString(Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            int len = ReadInt32();
            if (len == -1)
                return null;

            byte[] data = ReadBytes(len);
            return encoding.GetString(data, 0, len);
        }

        public decimal ReadDecimal()
        {
            int len = ReadInt32();
            int[] bits = new int[len];
            for (int i = 0; i < bits.Length; ++i)
                bits[i] = ReadInt32();

            return new decimal(bits);
        }

        public float ReadSingle()
        {
            return BitConverter.ToSingle(ReadBytes(sizeof(float)), 0);
        }

        public double ReadDouble()
        {
            return BitConverter.ToDouble(ReadBytes(sizeof(double)), 0);
        }

        public void Flush()
        {
            this.stream.Flush();
        }
    }
}
