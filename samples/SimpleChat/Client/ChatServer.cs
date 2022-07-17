using Drastic.Tempest;
using System.Collections.Generic;

namespace SimpleChat.Client
{
    public sealed class ChatServer
        : TempestServer
    {
        public ChatServer(IConnectionProvider provider)
            : base(provider, MessageTypes.Reliable)
        {
            this.RegisterMessageHandler<ChatMessage>(OnChatMessage);
        }

        private readonly List<IConnection> connections = new List<IConnection>();
        private void OnChatMessage(MessageEventArgs<ChatMessage> e)
        {
            ChatMessage msg = e.Message;

            // Messages come in on various threads, we'll need to make
            // sure we stay thread safe.
            lock (this.connections)
            {
                foreach (IConnection connection in this.connections)
                    connection.SendAsync(e.Message);
            }
        }

        protected override void OnConnectionMade(object sender, ConnectionMadeEventArgs e)
        {
            lock (this.connections)
                this.connections.Add(e.Connection);

            base.OnConnectionMade(sender, e);
        }

        protected override void OnConnectionDisconnected(object sender, DisconnectedEventArgs e)
        {
            lock (this.connections)
                this.connections.Remove(e.Connection);

            base.OnConnectionDisconnected(sender, e);
        }
    }
}
