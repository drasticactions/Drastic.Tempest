﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Tempest
{
    public static class SerializerExtensions
    {
        public static void Write7BitEncodedInt(this IValueWriter writer, int value)
        {
            uint v = (uint)value;
            while (v >= 128)
            {
                writer.WriteByte((byte)(v | 128));
                v >>= 7;
            }

            writer.WriteByte((byte)v);
        }

        public static int Read7BitEncodedInt(this IValueReader reader)
        {
            int count = 0;
            int shift = 0;
            byte b;

            do
            {
                b = reader.ReadByte();

                count |= (b & 127) << shift;
                shift += 7;
            } while ((b & 128) != 0);

            return count;
        }

        public static void WriteUniversalDate(this IValueWriter writer, DateTime date)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            if (date.Kind != DateTimeKind.Utc)
                date = date.ToUniversalTime();

            writer.WriteInt64(date.Ticks);
        }

        public static DateTime ReadUniversalDate(this IValueReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new DateTime(reader.ReadInt64(), DateTimeKind.Utc);
        }

        /// <summary>
		/// Writes a date value.
		/// </summary>
		public static void WriteDate(this IValueWriter writer, DateTime date)
        {
            writer.WriteInt64(date.Ticks);
        }

        /// <summary>
        /// Reads a date value.
        /// </summary>
        public static DateTime ReadDate(this IValueReader reader)
        {
            return new DateTime(reader.ReadInt64(), DateTimeKind.Unspecified);
        }

        public static void WriteLocalDate(this IValueWriter writer, DateTime date)
        {
            WriteLocalDate(writer, date, TimeZoneInfo.Local);
        }

        public static void WriteLocalDate(this IValueWriter writer, DateTime date, TimeZoneInfo timeZone)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteString(timeZone.ToSerializedString());
            writer.WriteInt64(date.ToLocalTime().Ticks);
        }

        public static Tuple<TimeZoneInfo, DateTime> ReadLocalDate(this IValueReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new Tuple<TimeZoneInfo, DateTime>(TimeZoneInfo.FromSerializedString(reader.ReadString()),
                                                      new DateTime(reader.ReadInt64(), DateTimeKind.Unspecified));
        }

        public static void WriteString(this IValueWriter writer, string value)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteString(Encoding.UTF8, value);
        }

        public static string ReadString(this IValueReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return reader.ReadString(Encoding.UTF8);
        }

        public static void WriteEnumerable<T>(this IValueWriter writer, ISerializationContext context, IEnumerable<T> enumerable)
            where T : ISerializable
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            T[] elements = enumerable.ToArray();
            writer.WriteInt32(elements.Length);
            for (int i = 0; i < elements.Length; ++i)
                elements[i].Serialize(context, writer);
        }

        public static void WriteEnumerable<T>(this IValueWriter writer, ISerializationContext context, ISerializer<T> serializer, IEnumerable<T> enumerable)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (serializer == null)
                throw new ArgumentNullException("serializer");
            if (enumerable == null)
                throw new ArgumentNullException("enumerable");

            T[] elements = enumerable.ToArray();
            writer.WriteInt32(elements.Length);
            for (int i = 0; i < elements.Length; ++i)
                serializer.Serialize(context, writer, elements[i]);
        }

        public static IEnumerable<T> ReadEnumerable<T>(this IValueReader reader, ISerializationContext context, Func<T> elementFactory)
            where T : ISerializable
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (elementFactory == null)
                throw new ArgumentNullException("elementFactory");

            int length = reader.ReadInt32();
            T[] elements = new T[length];
            for (int i = 0; i < elements.Length; ++i)
                (elements[i] = elementFactory()).Deserialize(context, reader);

            return elements;
        }

        public static IEnumerable<T> ReadEnumerable<T>(this IValueReader reader, ISerializationContext context, ISerializer<T> serializer)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (context == null)
                throw new ArgumentNullException("context");
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            int length = reader.ReadInt32();
            T[] elements = new T[length];
            for (int i = 0; i < elements.Length; ++i)
                elements[i] = serializer.Deserialize(context, reader);

            return elements;
        }

        public static IEnumerable<T> ReadEnumerable<T>(this IValueReader reader, ISerializationContext context, Func<ISerializationContext, IValueReader, T> elementFactory)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (context == null)
                throw new ArgumentNullException("context");
            if (elementFactory == null)
                throw new ArgumentNullException("elementFactory");

            int length = reader.ReadInt32();
            T[] elements = new T[length];
            for (int i = 0; i < elements.Length; ++i)
                elements[i] = elementFactory(context, reader);

            return elements;
        }

        public static void Write(this IValueWriter writer, ISerializationContext context, object element, Type serializeAs)
        {
            Write(writer, context, element, new FixedSerializer(serializeAs));
        }

        public static void Write(this IValueWriter writer, ISerializationContext context, object element, ISerializer serializer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (element == null)
                throw new ArgumentNullException("element");
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            serializer.Serialize(context, writer, element);
        }

        public static void Write<T>(this IValueWriter writer, ISerializationContext context, T element)
        {
            Write(writer, context, element, Serializer<T>.Default);
        }

        public static void Write<T>(this IValueWriter writer, ISerializationContext context, T element, ISerializer<T> serializer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            serializer.Serialize(context, writer, element);
        }

        public static T Read<T>(this IValueReader reader, ISerializationContext context)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return Read(reader, context, Serializer<T>.Default);
        }

        public static T Read<T>(this IValueReader reader, ISerializationContext context, ISerializer<T> serializer)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            if (serializer == null)
                throw new ArgumentNullException("serializer");

            return serializer.Deserialize(context, reader);
        }

        public static object Read(this IValueReader reader, ISerializationContext context)
        {
            return Read<object>(reader, context);
        }

        public static object Read(this IValueReader reader, ISerializationContext context, Type type)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return ObjectSerializer.GetSerializer(type).Deserialize(context, reader);
        }
    }
}
