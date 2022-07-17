// <copyright file="TempestMessage.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.InternalProtocol
{
    /// <summary>
    /// Base class for all internal Tempest protocol messages.
    /// </summary>
    public abstract class TempestMessage
        : Message
    {
        protected TempestMessage(TempestMessageType type)
            : base(InternalProtocol, (ushort)type)
        {
        }

        public static readonly Protocol InternalProtocol = new Protocol(0, 2) { id = 1 }; // Error check bypass hack
        static TempestMessage()
        {
            InternalProtocol.Register(new[]
            {
                new KeyValuePair<Type, Func<Message>> (typeof(PingMessage), () => new PingMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof(PongMessage), () => new PongMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof(DisconnectMessage), () => new DisconnectMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof(ConnectMessage), () => new ConnectMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof(AcknowledgeConnectMessage), () => new AcknowledgeConnectMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof(FinalConnectMessage), () => new FinalConnectMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof(ConnectedMessage), () => new ConnectedMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof(AcknowledgeMessage), () => new AcknowledgeMessage()),
                new KeyValuePair<Type, Func<Message>> (typeof(PartialMessage), () => new PartialMessage()),
            });
        }
    }
}
