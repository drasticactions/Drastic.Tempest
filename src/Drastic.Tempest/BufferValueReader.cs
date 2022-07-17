// <copyright file="BufferValueReader.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Runtime.InteropServices;
using System.Text;
using Buff = System.Buffer;

namespace Drastic.Tempest
{
    public unsafe class BufferValueReader
        : IValueReader, IDisposable
    {
        private readonly byte[] buffer;
        private readonly int length;

        public BufferValueReader(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            this.buffer = buffer;
            this.length = buffer.Length;

            this.handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            this.ptr = (byte*)this.handle.AddrOfPinnedObject();
        }

        public BufferValueReader(byte[] buffer, int offset, int length)
            : this(buffer)
        {
            this.ptr += offset;
            this.length = length;
        }

        /// <summary>
        /// Gets the underlying buffer.
        /// </summary>
        public byte[] Buffer
        {
            get { return this.buffer; }
        }

        /// <summary>
        /// Gets or sets the position of the reader in the buffer.
        /// </summary>
        public int Position
        {
            get { return (int)((ulong)this.ptr - (ulong)this.handle.AddrOfPinnedObject()); }
            set { this.ptr = (byte*)(this.handle.AddrOfPinnedObject() + value); }
        }

        public bool ReadBool()
        {
            return (*this.ptr++ == 1);
        }

        public byte[] ReadBytes()
        {
            int len = ReadInt32();

            byte[] b = new byte[len];
            Buff.BlockCopy(this.buffer, this.Position, b, 0, len);
            this.Position += len;

            return b;
        }

        public byte[] ReadBytes(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "count must be >= 0");

            byte[] b = new byte[count];
            Buff.BlockCopy(this.buffer, Position, b, 0, count);
            Position += count;

            return b;
        }

        public sbyte ReadSByte()
        {
            return *(sbyte*)this.ptr++;
        }

        public short ReadInt16()
        {
            short v = *(short*)this.ptr;
            this.ptr += 2;
            return v;
        }

        public int ReadInt32()
        {
            int v = *((int*)(this.ptr));
            this.ptr += sizeof(int);
            return v;
        }

        public long ReadInt64()
        {
            long v = *(long*)this.ptr;
            this.ptr += sizeof(long);
            return v;
        }

        public byte ReadByte()
        {
            return *this.ptr++;
        }

        public ushort ReadUInt16()
        {
            ushort v = *(ushort*)this.ptr;
            this.ptr += sizeof(ushort);
            return v;
        }

        public uint ReadUInt32()
        {
            uint v = *(uint*)this.ptr;
            this.ptr += sizeof(uint);
            return v;
        }

        public ulong ReadUInt64()
        {
            ulong v = *((ulong*)this.ptr);
            this.ptr += sizeof(long);
            return v;
        }

        public decimal ReadDecimal()
        {
            int[] parts = new int[4];
            for (int i = 0; i < parts.Length; ++i)
                parts[i] = ReadInt32();

            return new decimal(parts);
        }

        public float ReadSingle()
        {
            float v = *(float*)this.ptr;
            this.ptr += sizeof(float);
            return v;
        }

        public double ReadDouble()
        {
            double v = *(double*)this.ptr;
            this.ptr += sizeof(double);
            return v;
        }

        public string ReadString(Encoding encoding)
        {
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            int len = this.Read7BitEncodedInt();
            if (len == -1)
                return null;

            string v = encoding.GetString(this.buffer, Position, len);
            Position += len;

            return v;
        }

        public void Flush()
        {
        }

        public void Dispose()
        {
#if !SAFE
            this.handle.Free();
#endif
        }

#if !SAFE
        private GCHandle handle;
        private byte* ptr;
#else
		private int position;
#endif
    }
}
