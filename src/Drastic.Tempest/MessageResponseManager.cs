﻿// <copyright file="MessageResponseManager.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Collections.Concurrent;

namespace Drastic.Tempest
{
    public sealed class MessageResponseManager
    {
        public Task<Message> SendFor(Message message, Func<Message, Task<bool>> sender)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (message.Header == null)
                throw new ArgumentNullException("message", "Message.Header is null");

            return SendForCore(message, sender);
        }

        public Task<Message> SendFor(Message message, Func<Message, Task<bool>> sender, int timeout)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (message.Header == null)
                throw new ArgumentNullException("message", "Message.Header is null");

            if (timeout > 0)
            {
                if (!this.timeouts.TryAdd(message.Header.MessageId, DateTime.Now + TimeSpan.FromMilliseconds(timeout)))
                    throw new InvalidOperationException("Message already waiting response");
            }

            return SendForCore(message, sender);
        }

        public Task<Message> SendFor(Message message, Func<Message, Task<bool>> sender, CancellationToken cancelToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (message.Header == null)
                throw new ArgumentNullException("message", "Message.Header is null");
            if (sender == null)
                throw new ArgumentNullException("sender");

            int id = message.Header.MessageId;
            cancelToken.Register(() => {
                TaskCompletionSource<Message> cancelTcs;
                if (this.messageResponses.TryRemove(id, out cancelTcs))
                    cancelTcs.TrySetCanceled();
            });

            return SendForCore(message, sender);
        }

        public Task<Message> SendFor(Message message, Task<bool> sendTask)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (message.Header == null)
                throw new ArgumentNullException("message", "Message.Header is null");

            return SendForCore(message, sendTask: sendTask);
        }

        public Task<Message> SendFor(Message message, Task<bool> sendTask, int timeout)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (message.Header == null)
                throw new ArgumentNullException("message", "Message.Header is null");

            if (timeout > 0)
            {
                if (!this.timeouts.TryAdd(message.Header.MessageId, DateTime.Now + TimeSpan.FromMilliseconds(timeout)))
                    throw new InvalidOperationException("Message already waiting response");
            }

            return SendForCore(message, sendTask: sendTask);
        }

        public Task<Message> SendFor(Message message, Task<bool> sendTask, CancellationToken cancelToken)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (message.Header == null)
                throw new ArgumentNullException("message", "Message.Header is null");
            if (sendTask == null)
                throw new ArgumentNullException("sendTask");

            int id = message.Header.MessageId;
            cancelToken.Register(() => {
                TaskCompletionSource<Message> cancelTcs;
                if (this.messageResponses.TryRemove(id, out cancelTcs))
                    cancelTcs.TrySetCanceled();
            });

            return SendForCore(message, sendTask: sendTask);
        }

        public void Receive(Message message)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            if (message.Header == null)
                throw new ArgumentNullException("message", "Message.Header is null");
            if (!message.Header.IsResponse)
                throw new ArgumentException("message is not a response message", "message");

            TaskCompletionSource<Message> tcs;
            if (!this.messageResponses.TryRemove(message.Header.ResponseMessageId, out tcs))
                return;

            tcs.TrySetResult(message);

            DateTime timeout;
            this.timeouts.TryRemove(message.Header.MessageId, out timeout);
        }

        public void Clear()
        {
            this.timeouts.Clear();

            foreach (var kvp in messageResponses)
                kvp.Value.TrySetCanceled();

            this.messageResponses.Clear();
        }

        /// <summary>
        /// Checks for any message responses that have timed out and cancels them.
        /// </summary>
        public void CheckTimeouts()
        {
            var now = DateTime.Now;
            List<int> remove = new List<int>();

            foreach (var kvp in this.timeouts)
            {
                if (now < kvp.Value)
                    continue;

                TaskCompletionSource<Message> tcs;
                if (this.messageResponses.TryGetValue(kvp.Key, out tcs))
                    tcs.TrySetCanceled();

                remove.Add(kvp.Key);
            }

            foreach (int id in remove)
            {
                TaskCompletionSource<Message> tcs;
                DateTime timeout;

                this.timeouts.TryRemove(id, out timeout);
                this.messageResponses.TryRemove(id, out tcs);
            }
        }

        private readonly ConcurrentDictionary<int, TaskCompletionSource<Message>> messageResponses = new ConcurrentDictionary<int, TaskCompletionSource<Message>>();
        private readonly ConcurrentDictionary<int, DateTime> timeouts = new ConcurrentDictionary<int, DateTime>();

        private Task<Message> SendForCore(Message message, Func<Message, Task<bool>> sender = null, Task<bool> sendTask = null)
        {
            int id = message.Header.MessageId;

            var tcs = new TaskCompletionSource<Message>(message);

            if (!this.messageResponses.TryAdd(id, tcs))
                throw new InvalidOperationException("Message already waiting response");

            if (sender != null)
                sendTask = sender(message);

            sendTask.ContinueWith(t => {
                if (!t.IsFaulted && t.Result)
                    return;

                TaskCompletionSource<Message> cancelTcs;
                if (this.messageResponses.TryRemove(id, out cancelTcs))
                    cancelTcs.TrySetCanceled();
            }, TaskScheduler.Default);

            return tcs.Task;
        }
    }
}
