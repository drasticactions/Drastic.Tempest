// <copyright file="Target.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public sealed class Target
            : IEquatable<Target>, ISerializable
    {
        public const string AnyIP = "0.0.0.0";
        public const string AnyIPv6 = "::";
        public const string LoopbackIP = "127.0.0.1";
        public const string LoopbackIPv6 = "[::1]";

        public Target(string hostname, int port)
        {
            if (hostname == null)
                throw new ArgumentNullException("hostname");

            Hostname = hostname;
            Port = port;
        }

        public Target(ISerializationContext context, IValueReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            Deserialize(context, reader);
        }

        public string Hostname
        {
            get;
            private set;
        }

        public int Port
        {
            get;
            private set;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;

            return obj is Target && Equals((Target)obj);
        }

        public bool Equals(Target other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return (String.Equals(Hostname, other.Hostname) && Port == other.Port);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Hostname.GetHashCode() * 397) ^ Port;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Hostname, Port);
        }

        public static bool operator ==(Target left, Target right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Target left, Target right)
        {
            return !Equals(left, right);
        }

        public void Serialize(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteString(Hostname);
            writer.WriteInt32(Port);
        }

        public void Deserialize(ISerializationContext context, IValueReader reader)
        {
            Hostname = reader.ReadString();
            Port = reader.ReadInt32();
        }
    }
}
