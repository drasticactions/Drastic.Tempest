// <copyright file="ConnectMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.InternalProtocol
{
    public sealed class ConnectMessage
            : TempestMessage
    {
        public ConnectMessage()
            : base(TempestMessageType.Connect)
        {
        }

        /// <summary>
        /// Gets or sets the hashing algorithms available for signing.
        /// </summary>
        public IEnumerable<string> SignatureHashAlgorithms
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the protocols (and versions of protocols) to connect for.
        /// </summary>
        public IEnumerable<Protocol> Protocols
        {
            get;
            set;
        }

        public override bool AcceptedConnectionlessly
        {
            get { return true; }
        }

        public override void WritePayload(ISerializationContext context, IValueWriter writer)
        {
            if (writer.WriteBool(SignatureHashAlgorithms != null))
            {
                string[] algs = SignatureHashAlgorithms.ToArray();
                writer.WriteInt32(algs.Length);
                for (int i = 0; i < algs.Length; ++i)
                    writer.WriteString(algs[i]);
            }

            Protocol[] protocols = Protocols.ToArray();
            writer.WriteInt32(protocols.Length);
            for (int i = 0; i < protocols.Length; ++i)
                protocols[i].Serialize(context, writer);
        }

        public override void ReadPayload(ISerializationContext context, IValueReader reader)
        {
            if (reader.ReadBool())
            {
                string[] algs = new string[reader.ReadInt32()];
                for (int i = 0; i < algs.Length; ++i)
                    algs[i] = reader.ReadString();

                SignatureHashAlgorithms = algs;
            }

            Protocol[] protocols = new Protocol[reader.ReadInt32()];
            for (int i = 0; i < protocols.Length; ++i)
                protocols[i] = new Protocol(context, reader);

            Protocols = protocols;
        }
    }
}
