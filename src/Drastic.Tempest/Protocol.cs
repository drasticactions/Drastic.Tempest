// <copyright file="Protocol.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Identifies a messaging protocol.
    /// </summary>
    /// <remarks>
    /// Multiple Tempest-build libraries and applications can run on a single
    /// set of connection provider and connections. Protocols are used to
    /// identify the various sets of messages so that the correct handlers
    /// receive the correct messages.
    /// </remarks>
    public sealed class Protocol
        : MessageFactory, IEquatable<Protocol>, ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Protocol"/> class.
        /// </summary>
        /// <param name="id">The ID of the protocol.</param>
        /// <exception cref="ArgumentException"><paramref name="id"/> is 1.</exception>
        /// <remarks>
        /// Protocol ID 1 is reserved for internal Tempest use.
        /// </remarks>
        public Protocol(byte id)
        {
            if (id == 1)
                throw new ArgumentException("ID 1 is reserved for Tempest use.", "id");

            this.id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Protocol"/> class.
        /// </summary>
        /// <param name="id">The ID of the protocol.</param>
        /// <param name="version">The version of the protocol.</param>
        /// <exception cref="ArgumentException"><paramref name="id"/> is 1.</exception>
        /// <remarks>
        /// Protocol ID 1 is reserved for internal Tempest use.
        /// </remarks>
        public Protocol(byte id, int version)
            : this(id)
        {
            this.version = version;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Protocol"/> class.
        /// </summary>
        /// <param name="id">The ID of the protocol.</param>
        /// <param name="version">The version of the protocol.</param>
        /// <param name="compatibleVersions">Versions of this protcol that are compatible with this version.</param>
        /// <exception cref="ArgumentException"><paramref name="id"/> is 1.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="compatibleVersions"/> is <c>null</c></exception>
        /// <remarks>
        /// Protocol ID 1 is reserved for internal Tempest use.
        /// </remarks>
        public Protocol(byte id, int version, params int[] compatibleVersions)
            : this(id, version)
        {
            if (compatibleVersions == null)
                throw new ArgumentNullException("compatibleVersions");

            this.compatible = compatibleVersions.ToDictionary(v => v, v => true);
        }

        internal Protocol(ISerializationContext context, IValueReader reader)
        {
            Deserialize(context, reader);
        }

        /// <summary>
        /// Gets the unique identifier for this protocol.
        /// </summary>
        public byte Id
        {
            get { return this.id; }
        }

        /// <summary>
        /// Gets the version of this protocol.
        /// </summary>
        public int Version
        {
            get { return this.version; }
        }

        /// <summary>
        /// Gets whether <paramref name="versionToCheck"/> is compatible with this version.
        /// </summary>
        /// <param name="versionToCheck">The version to check against this version.</param>
        /// <returns><c>true</c> if compatible, <c>false</c> if not.</returns>
        public bool CompatibleWith(int versionToCheck)
        {
            if (versionToCheck == this.version)
                return true;
            if (this.compatible == null)
                return false;

            return this.compatible.ContainsKey(versionToCheck);
        }

        /// <summary>
        /// Gets whether <paramref name="protocol"/> is compatible with this version.
        /// </summary>
        /// <param name="protocol">The protocol to check against this version.</param>
        /// <returns><c>true</c> if compatible, <c>false</c> if not.</returns>
        public bool CompatibleWith(Protocol protocol)
        {
            if (protocol == null)
                throw new ArgumentNullException("protocol");

            if (this.id != protocol.id)
                return false;

            return CompatibleWith(protocol.Version);
        }

        /// <summary>
        /// Gets whether <paramref name="protocol" /> is the same protocol (ignoring version).
        /// </summary>
        /// <param name="protocol">Protocl to check.</param>
        /// <returns><c>true</c> if the protocol id's match, <c>false</c> otherwise.</returns>
        public bool IsSameProtocolAs(Protocol protocol)
        {
            if (protocol == null)
                throw new ArgumentNullException("protocol");

            return id == protocol.id;
        }

        public void Serialize(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteByte(this.id);
            writer.WriteInt32(this.version);
        }

        public void Deserialize(ISerializationContext context, IValueReader reader)
        {
            this.id = reader.ReadByte();
            this.version = reader.ReadInt32();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Protocol);
        }

        public bool Equals(Protocol other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return (GetType() == other.GetType() && other.id == id && version == other.version);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode() ^ version.GetHashCode() ^ GetType().GetHashCode();
        }

        public static bool operator ==(Protocol left, Protocol right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Protocol left, Protocol right)
        {
            return !Equals(left, right);
        }

        private int version;
        internal byte id;

        /// <remarks>WP7 doesn't have HashSet.</remarks>
        private readonly Dictionary<int, bool> compatible;
    }
}
