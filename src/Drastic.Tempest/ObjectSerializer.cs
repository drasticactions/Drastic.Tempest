// <copyright file="ObjectSerializer.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Reflection;
using System.Text;

namespace Drastic.Tempest
{
    internal class ObjectSerializer
    {
        private readonly Type type;

        public ObjectSerializer(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this.type = type;

            GenerateSerialization();
        }

        public void Serialize(ISerializationContext context, IValueWriter writer, object obj)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            serializer(context, writer, obj, false);
        }

        public T Deserialize<T>(ISerializationContext context, IValueReader reader)
        {
            if (typeof(T) != this.type)
                throw new ArgumentException("Type does not match serializer type");

            return (T)Deserialize(context, reader);
        }

        public object Deserialize(ISerializationContext context, IValueReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return deserializer(context, reader, false);
        }

        private Func<ISerializationContext, IValueReader, bool, object> deserializer;
        private Action<ISerializationContext, IValueWriter, object, bool> serializer;

        private void GenerateSerialization()
        {
            deserializer = GetDeserializer(this.type, this);
            serializer = GetSerializer();
        }

        private class SerializationPair
        {
            public readonly Func<ISerializationContext, IValueReader, bool, object> Deserializer;
            public readonly Action<ISerializationContext, IValueWriter, object, bool> Serializer;

            public SerializationPair(Func<ISerializationContext, IValueReader, bool, object> des, Action<ISerializationContext, IValueWriter, object, bool> ser)
            {
                Deserializer = des;
                Serializer = ser;
            }
        }

        private ConstructorInfo ctor;
        private Dictionary<MemberInfo, SerializationPair> members;
        private bool deserializingConstructor;

        private static Func<ISerializationContext, IValueReader, bool, object> GetDeserializer(Type t, ObjectSerializer oserializer)
        {
            var ti = t.GetTypeInfo();
            if (ti.IsPrimitive)
            {
                if (t == typeof(bool))
                    return (c, r, sh) => r.ReadBool();
                if (t == typeof(byte))
                    return (c, r, sh) => r.ReadByte();
                if (t == typeof(sbyte))
                    return (c, r, sh) => r.ReadSByte();
                if (t == typeof(short))
                    return (c, r, sh) => r.ReadInt16();
                if (t == typeof(ushort))
                    return (c, r, sh) => r.ReadUInt16();
                if (t == typeof(int))
                    return (c, r, sh) => r.ReadInt32();
                if (t == typeof(uint))
                    return (c, r, sh) => r.ReadUInt32();
                if (t == typeof(long))
                    return (c, r, sh) => r.ReadInt64();
                if (t == typeof(ulong))
                    return (c, r, sh) => r.ReadUInt64();
                if (t == typeof(float))
                    return (c, r, sh) => r.ReadSingle();
                if (t == typeof(double))
                    return (c, r, sh) => r.ReadDouble();

                throw new ArgumentOutOfRangeException("type"); // Shouldn't happen.
            }
            else if (t == typeof(decimal))
                return (c, r, sh) => r.ReadDecimal();
            else if (t == typeof(DateTime))
                return (c, r, sh) => r.ReadDate();
            else if (t == typeof(string))
                return (c, r, sh) => !r.ReadBool() ? null : r.ReadString(Encoding.UTF8);
            else if (ti.IsEnum)
            {
                Type btype = Enum.GetUnderlyingType(t);
                return GetDeserializer(btype, oserializer);
            }
            else if (t.IsArray || t == typeof(Array))
            {
                Type etype = t.GetElementType();

                return (c, r, sh) =>
                {
                    if (!r.ReadBool())
                        return null;

                    Array a = Array.CreateInstance(etype, r.ReadInt32());
                    for (int i = 0; i < a.Length; ++i)
                        a.SetValue(r.Read(c, etype), i);

                    return a;
                };
            }
            else
            {
                return (c, r, skipHeader) =>
                {
                    if (!skipHeader)
                    {
                        bool isLive = r.ReadBool();
                        if (!isLive)
                            return null;

                        return GetSerializer(t).deserializer(c, r, true);
                    }

                    object value;

                    if (typeof(Tempest.ISerializable).IsAssignableFrom(t))
                    {
                        oserializer.LoadCtor(t);
                        if (!oserializer.deserializingConstructor)
                        {
                            value = oserializer.ctor.Invoke(null);
                            ((Tempest.ISerializable)value).Deserialize(c, r);
                        }
                        else
                            value = oserializer.ctor.Invoke(new object[] { c, r });

                        return value;
                    }

                    throw new ArgumentException("No serializer found for type " + t);
                };
            }
        }

        private Action<ISerializationContext, IValueWriter, object, bool> GetSerializer()
        {
            return GetSerializerAction(this.type);
        }

        private Action<ISerializationContext, IValueWriter, object, bool> GetSerializerAction(Type t)
        {
            var ti = t.GetTypeInfo();
            if (ti.IsPrimitive)
            {
                if (t == typeof(bool))
                    return (c, w, v, sh) => w.WriteBool((bool)v);
                if (t == typeof(byte))
                    return (c, w, v, sh) => w.WriteByte((byte)v);
                else if (t == typeof(sbyte))
                    return (c, w, v, sh) => w.WriteSByte((sbyte)v);
                else if (t == typeof(short))
                    return (c, w, v, sh) => w.WriteInt16((short)v);
                else if (t == typeof(ushort))
                    return (c, w, v, sh) => w.WriteUInt16((ushort)v);
                else if (t == typeof(int))
                    return (c, w, v, sh) => w.WriteInt32((int)v);
                else if (t == typeof(uint))
                    return (c, w, v, sh) => w.WriteUInt32((uint)v);
                else if (t == typeof(long))
                    return (c, w, v, sh) => w.WriteInt64((long)v);
                else if (t == typeof(ulong))
                    return (c, w, v, sh) => w.WriteUInt64((ulong)v);
                else if (t == typeof(float))
                    return (c, w, v, sh) => w.WriteSingle((float)v);
                else if (t == typeof(double))
                    return (c, w, v, sh) => w.WriteDouble((double)v);

                throw new ArgumentOutOfRangeException("type"); // Shouldn't happen.
            }
            else if (t == typeof(decimal))
                return (c, w, v, sh) => w.WriteDecimal((decimal)v);
            else if (t == typeof(DateTime))
                return (c, w, v, sh) => w.WriteDate((DateTime)(object)v);
            else if (t == typeof(string))
            {
                return (c, w, v, sh) =>
                {
                    w.WriteBool(v != null);
                    if (v != null)
                        w.WriteString(Encoding.UTF8, (string)v);
                };
            }
            else if (ti.IsEnum)
            {
                Type btype = Enum.GetUnderlyingType(t);
                return GetSerializerAction(btype);
            }
            else if (t.IsArray || t == typeof(Array))
            {
                return (c, w, v, sh) =>
                {
                    if (v == null)
                    {
                        w.WriteBool(false);
                        return;
                    }

                    w.WriteBool(true);

                    Array a = (Array)v;
                    w.WriteInt32(a.Length);

                    if (ti.IsPrimitive)
                    {
                        for (int i = 0; i < a.Length; ++i)
                            w.Write(c, a.GetValue(i));
                    }
                    else
                    {
                        var etype = t.GetElementType();
                        for (int i = 0; i < a.Length; ++i)
                            w.Write(c, a.GetValue(i), etype);
                    }
                };
            }
            else
            {
                return (c, w, v, sh) =>
                {
                    if (!sh)
                    {
                        if (v == null)
                        {
                            w.WriteBool(false);
                            return;
                        }

                        Type actualType = v.GetType();
                        w.WriteBool(true);

                        if (!t.IsAssignableFrom(actualType))
                            throw new ArgumentException();

                        if (actualType != t)
                        {
                            GetSerializer(actualType).serializer(c, w, v, true);
                            return;
                        }
                    }

                    var serializable = (v as Tempest.ISerializable);
                    if (serializable != null)
                    {
                        serializable.Serialize(c, w);
                        return;
                    }

                    throw new ArgumentException("No serializer found or specified for type " + t, "value");
                };
            }
        }

        private void LoadCtor(Type t)
        {
            if (this.ctor != null)
                return;

#if !NETFX_CORE
            this.ctor = t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, new[] { typeof(ISerializationContext), typeof(IValueReader) }, null)
                        ?? t.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null);
#else
			this.ctor = t.GetConstructor (new[] { typeof (ISerializationContext), typeof (IValueReader) })
						?? t.GetConstructor (new Type[0]);
#endif

            if (this.ctor == null)
                throw new ArgumentException("No empty or (ISerializationContext,IValueReader) constructor found for " + t.Name);

            this.deserializingConstructor = this.ctor.GetParameters().Length == 2;
        }

        private void Serialize(ISerializationContext context, IValueWriter writer, object value, bool skipHeader)
        {
            this.serializer(context, writer, value, skipHeader);
        }

        private object Deserialize(ISerializationContext context, IValueReader reader, bool skipHeader)
        {
            return this.deserializer(context, reader, skipHeader);
        }

        private static readonly ConcurrentDictionary<Type, ObjectSerializer> Serializers = new ConcurrentDictionary<Type, ObjectSerializer>();

        internal ObjectSerializer GetSerializerInternal(Type stype)
        {
            if (this.type == stype)
                return this;

            return GetSerializer(stype);
        }

        private static readonly ObjectSerializer baseSerializer = new ObjectSerializer(typeof(object));

        internal static ObjectSerializer GetSerializer(Type type)
        {
            var ti = type.GetTypeInfo();
            if (type == typeof(object) || ti.IsInterface || ti.IsAbstract)
                return baseSerializer;

            return Serializers.GetOrAdd(type, t => new ObjectSerializer(t));
        }
    }
}
