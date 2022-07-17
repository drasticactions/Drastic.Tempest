// <copyright file="MockServerConnection.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public class MockServerConnection
            : MockConnection, IServerConnection
    {
        private MockClientConnection connection;

        internal MockServerConnection(MockClientConnection connection)
        {
            this.connected = true;
            this.connection = connection;
        }

        public override Task<bool> SendAsync(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (!IsConnected)
            {
                TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
                tcs.SetResult(false);
                return tcs.Task;
            }

            Task<bool> task = base.SendAsync(message);
            this.connection.Receive(new MessageEventArgs(this.connection, message));
            return task;
        }

        public override Task<bool> SendResponseAsync(Message originalMessage, Message response)
        {
            var tcs = new TaskCompletionSource<bool>();

            // Sometimes we manually construct messages to test handlers, we'll go ahead and build a header
            // for those automatically to save ourselves time.
            PrepareMessage(originalMessage);

            PrepareMessage(response);
            response.Header.ResponseMessageId = originalMessage.Header.MessageId;
            response.Header.IsResponse = true;

            this.connection.ReceiveResponse(new MessageEventArgs(this.connection, response));

            tcs.SetResult(true);
            return tcs.Task;
        }

        protected internal override Task Disconnect(ConnectionResult reason = ConnectionResult.FailedUnknown, string customReason = null)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            if (!this.connected)
            {
                tcs.SetResult(true);
            }
            else
            {
                base.Disconnect(reason, customReason).ContinueWith(t => {
                    var c = Interlocked.Exchange(ref this.connection, null);
                    if (c != null)
                        c.Disconnect(reason, customReason).ContinueWith(t2 => tcs.SetResult(true));
                    else
                        tcs.SetResult(false);
                });
            }

            return tcs.Task;
        }
    }
}
