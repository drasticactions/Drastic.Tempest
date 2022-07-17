// <copyright file="IValueWriter.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Text;

namespace Drastic.Tempest
{
    /// <summary>
    /// Serialization contract.
    /// </summary>
    public interface IValueWriter
    {
        /// <summary>
        /// Writes an unsigned byte to the transport.
        /// </summary>
        /// <param name="value"></param>
        void WriteByte(byte value);

        /// <summary>
        /// Writes a signed byte to the transport.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteSByte(sbyte value);

        /// <summary>
        /// Writes a boolean to the transport.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <returns><paramref name="value"/></returns>
        bool WriteBool(bool value);

        /// <summary>
        /// Writes an array of unsigned bytes to the transport.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        void WriteBytes(byte[] value);

        /// <summary>
        /// Writes an array segment to the transport.
        /// </summary>
        /// <param name="value">The array to write from.</param>
        /// <param name="offset">The offset of <paramref name="value"/> to start writing from.</param>
        /// <param name="length">The length to write from <paramref name="offset"/> in <paramref name="value"/>.</param>
        /// 
        void WriteBytes(byte[] value, int offset, int length);

        /// <summary>
        /// Writes a signed short (Int16) to the transport.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteInt16(Int16 value);

        /// <summary>
        /// Writes a signed integer (Int32) to the transport.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteInt32(Int32 value);

        /// <summary>
        /// Writes a signed long (Int64) to the transport.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteInt64(Int64 value);

        /// <summary>
        /// Writes an unsigned short to the transport.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteUInt16(UInt16 value);

        /// <summary>
        /// Writes an unsigned integer to the transport.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteUInt32(UInt32 value);

        /// <summary>
        /// Writes an unsigned long to the transport.
        /// </summary>
        /// <param name="value">The value to write.</param>
        void WriteUInt64(UInt64 value);

        /// <summary>
        /// Writes a decimal to the transport.
        /// </summary>
        /// <param name="value">The value to write</param>
        void WriteDecimal(Decimal value);

        /// <summary>
        /// Writes a single to the transport.
        /// </summary>
        /// <param name="value">The value to write</param>
        void WriteSingle(Single value);

        /// <summary>
        /// Writes a double to the transport.
        /// </summary>
        /// <param name="value">The value to write</param>
        void WriteDouble(Double value);

        /// <summary>
        /// Writes a string with <paramref name="encoding"/> to the transport.
        /// </summary>
        /// <param name="encoding">The encoding to use.</param>
        /// <param name="value">The value to write.</param>
        void WriteString(Encoding encoding, string value);

        /// <summary>
        /// Flushes any buffered data to the transport.
        /// </summary>
        void Flush();
    }
}
