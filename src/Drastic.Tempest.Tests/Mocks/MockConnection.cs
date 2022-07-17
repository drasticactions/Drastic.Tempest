﻿// <copyright file="MockConnection.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using Drastic.Tempest.InternalProtocol;

namespace Drastic.Tempest.Tests
{
    public abstract class MockConnection
            : IConnection
    {
        protected bool connected;

        public void Dispose()
        {
        }

        public bool IsConnected
        {
            get { return this.connected; }
        }

        public int ConnectionId
        {
            get;
            internal set;
        }

        public IEnumerable<Protocol> Protocols
        {
            get { throw new NotImplementedException(); }
        }

        public int ResponseTime
        {
            get { return -1; }
        }

        public Target RemoteTarget
        {
            get;
            private set;
        }

        public IAsymmetricKey RemoteKey
        {
            get { return new MockAsymmetricKey(); }
        }

        private MockAsymmetricKey key = new MockAsymmetricKey();
        public IAsymmetricKey LocalKey
        {
            get { return this.key; }
        }

        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<DisconnectedEventArgs> Disconnected;

        private int messageId;
        public virtual Task<bool> SendAsync(Message message)
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            PrepareMessage(message);

            return tcs.Task;
        }

        private readonly MessageResponseManager responses = new MessageResponseManager();

        public Task<Message> SendFor(Message message, int timeout = 0)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            PrepareMessage(message);

            return this.responses.SendFor(message, SendAsync, timeout);
        }

        public Task<TResponse> SendFor<TResponse>(Message message, int timeout = 0) where TResponse : Message
        {
            return SendFor(message, timeout).ContinueWith(t => (TResponse)t.Result, TaskScheduler.Default);
        }

        public abstract Task<bool> SendResponseAsync(Message originalMessage, Message response);

        public IEnumerable<MessageEventArgs> Tick()
        {
            throw new NotSupportedException();
        }

        public Task DisconnectAsync()
        {
            return DisconnectAsync(ConnectionResult.FailedUnknown);
        }

        public Task DisconnectAsync(ConnectionResult reason, string customReason = null)
        {
            return Disconnect(reason, customReason);
        }

        protected void PrepareMessage(Message message)
        {
            if (message.Header != null)
                return;

            message.Header = new MessageHeader();
            message.Header.MessageId = Interlocked.Increment(ref this.messageId);
        }

        protected internal virtual Task Disconnect(ConnectionResult reason = ConnectionResult.FailedUnknown, string customReason = null)
        {
            this.connected = false;

            Cleanup();

            var e = new DisconnectedEventArgs(this, reason, customReason);
            return Task.Factory.StartNew(s => OnDisconnected((DisconnectedEventArgs)s), e);
        }

        protected virtual void Cleanup()
        {
            this.responses.Clear();
        }

        internal void ReceiveResponse(MessageEventArgs e)
        {
            if (e.Message.Header.IsResponse)
                this.responses.Receive(e.Message);

            Receive(e);
        }

        internal void Receive(MessageEventArgs e)
        {
            var context = new SerializationContext();
            var writer = new BufferValueWriter(new byte[1024]);
            e.Message.WritePayload(context, writer);

            var reader = new BufferValueReader(writer.Buffer);
            var message = e.Message.Protocol.Create(e.Message.MessageType);
            message.ReadPayload(context, reader);
            message.Header = e.Message.Header;

            var tmessage = (e.Message as TempestMessage);
            if (tmessage == null)
                OnMessageReceived(new MessageEventArgs(e.Connection, message));
            else
                OnTempestMessageReceived(new MessageEventArgs(e.Connection, message));
        }

        protected virtual void OnTempestMessageReceived(MessageEventArgs e)
        {
            switch (e.Message.MessageType)
            {
                case (ushort)TempestMessageType.Ping:
                    SendAsync(new PongMessage());
                    break;

                case (ushort)TempestMessageType.Pong:
                    break;

                case (ushort)TempestMessageType.Disconnect:
                    var msg = (DisconnectMessage)e.Message;
                    DisconnectAsync(msg.Reason, msg.CustomReason);
                    break;
            }
        }

        protected void OnDisconnected(DisconnectedEventArgs e)
        {
            var handler = this.Disconnected;
            if (handler != null)
                handler(this, e);
        }

        protected void OnMessageReceived(MessageEventArgs e)
        {
            EventHandler<MessageEventArgs> handler = this.MessageReceived;
            if (handler != null)
                handler(this, e);
        }
    }
}
