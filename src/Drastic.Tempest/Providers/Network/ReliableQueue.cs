// <copyright file="ReliableQueue.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Providers.Network
{
    internal class ReliableQueue
    {
        public void Clear()
        {
            this.lastMessageInOrder = 0;
            lock (this.queue)
                this.queue.Clear();
        }

        public bool TryEnqueue(MessageEventArgs messageArgs, out List<MessageEventArgs> received)
        {
            received = null;

            var msg = messageArgs.Message;
            int mid = msg.Header.MessageId;

            // HACK: We should really disconnect these guys if they're sending +2000 messages
            if (mid <= this.lastMessageInOrder || mid > (this.lastMessageInOrder + 2000))
                return false;

            lock (this.queue)
            {
                int d = mid - this.lastMessageInOrder;
                if (d == 1)
                {
                    received = new List<MessageEventArgs>();
                    received.Add(messageArgs);

                    int i = 1;
                    for (; i < this.queue.Count; i++)
                    {
                        MessageEventArgs a = this.queue[i];
                        if (a == null)
                            break;

                        received.Add(a);
                    }

                    if (this.queue.Count > 0)
                        this.queue.RemoveRange(0, i);

                    this.lastMessageInOrder = received[received.Count - 1].Message.Header.MessageId;
                }
                else
                {
                    int i = -1;
                    for (int m = this.lastMessageInOrder; m < messageArgs.Message.Header.MessageId; m++)
                    {
                        if (++i >= this.queue.Count)
                            this.queue.Add(null);
                    }

                    if (i > this.queue.Count)
                        this.queue.Add(messageArgs);
                    else
                        this.queue[i] = messageArgs;
                }
            }

            return true;
        }

        // BUG: doesn't handle MID rollover
        private int lastMessageInOrder;
        private readonly List<MessageEventArgs> queue = new List<MessageEventArgs>();
    }
}
