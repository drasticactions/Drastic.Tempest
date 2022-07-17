// <copyright file="IValueReader.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Text;

namespace Drastic.Tempest
{
    public interface IValueReader
    {
        /// <summary>
        /// Reads a boolean from the transport.
        /// </summary>
        bool ReadBool();

        /// <summary>
        /// Reads an array of unsigned bytes from the transport.
        /// </summary>
        byte[] ReadBytes();

        /// <summary>
        /// Reads the next <paramref name="count"/> bytes from the transport.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is &lt; 0.</exception>
        byte[] ReadBytes(int count);

        /// <summary>
        /// Reads a signed byte (SByte) from the transport.
        /// </summary>
        SByte ReadSByte();

        /// <summary>
        /// Reads a signed short (Int16) from the transport.
        /// </summary>
        Int16 ReadInt16();

        /// <summary>
        /// Reads a signed integer (Int32) from the transport.
        /// </summary>
        Int32 ReadInt32();

        /// <summary>
        /// Reads a signed long (Int64) from the transport.
        /// </summary>
        Int64 ReadInt64();

        /// <summary>
        /// Reads an unsigned byte (Byte) from the transport.
        /// </summary>
        Byte ReadByte();

        /// <summary>
        /// Reads an unsigned integer (UInt16) from the transport.
        /// </summary>
        /// <returns></returns>
        UInt16 ReadUInt16();

        /// <summary>
        /// Reads an unsigned integer (UInt32) from the transport.
        /// </summary>
        /// <returns></returns>
        UInt32 ReadUInt32();

        /// <summary>
        /// Reads an unsigned long (UInt64) from the transport.
        /// </summary>
        UInt64 ReadUInt64();

        /// <summary>
        /// Reads a decimal from the transport.
        /// </summary>
        Decimal ReadDecimal();

        /// <summary>
        /// Reads a single from the transport.
        /// </summary>
        Single ReadSingle();

        /// <summary>
        /// Reads a double from the transport.
        /// </summary>
        Double ReadDouble();

        /// <summary>
        /// Reads a string with <paramref name="encoding"/> from the transport.
        /// </summary>
        /// <param name="encoding">The encoding of the string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
        string ReadString(Encoding encoding);

        /// <summary>
        /// Finalizes buffer.
        /// </summary>
        /// <remarks>Connection providers should call this automatically when <see cref="Message.ReadPayload"/> returns.</remarks>
        void Flush();
    }
}
