// <copyright file="BufferPool.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.Net.Sockets;

namespace Drastic.Tempest.Providers.Network
{
    internal sealed class BufferPool
    {
        public BufferPool(int defaultBufferSize, int bufferLimit)
            : this(defaultBufferSize)
        {
            if (bufferLimit <= 0)
                throw new ArgumentOutOfRangeException("bufferLimit", "Buffer limit must be >0");

            AutoSizeLimit = false;
            this.bufferLimit = bufferLimit;
        }

        public BufferPool(int defaultBufferSize)
        {
            if (defaultBufferSize <= 0)
                throw new ArgumentOutOfRangeException("defaultBufferSize", "Default buffer size must be >0");

            this.defaultBufferSize = defaultBufferSize;
            AutoSizeLimit = true;
            AutoSizeFactor = 2;
        }

        public int Limit
        {
            get { return this.bufferLimit; }
        }

        public bool AutoSizeLimit
        {
            get;
            set;
        }

        public int AutoSizeFactor
        {
            get;
            set;
        }

        public void AddConnection()
        {
            if (AutoSizeLimit)
                Interlocked.Add(ref this.bufferLimit, AutoSizeFactor);
        }

        public void RemoveConnection()
        {
            if (AutoSizeLimit)
                Interlocked.Add(ref this.bufferLimit, -AutoSizeFactor);
        }

        /// <summary>
        /// Tries to retrieve an existing buffer.
        /// </summary>
        /// <param name="args">The retrieved or created <see cref="SocketAsyncEventArgs"/>.</param>
        /// <returns><c>true</c> if the buffer was pre-existing, <c>false</c> if it was newly created.</returns>
        public bool TryGetBuffer(out SocketAsyncEventArgs args)
        {
            if (this.buffers.TryPop(out args))
                return true;

            SpinWait wait = new SpinWait();
            while (args == null)
            {
                int count = this.bufferCount;
                if (count == this.bufferLimit)
                {
                    if (this.buffers.TryPop(out args))
                        return true;
                }
                else if (count == Interlocked.CompareExchange(ref this.bufferCount, count + 1, count))
                {
                    args = new SocketAsyncEventArgs();
                    args.SetBuffer(new byte[this.defaultBufferSize], 0, this.defaultBufferSize);
                    return false;
                }

                wait.SpinOnce();
            }

            return true;
        }

        public void PushBuffer(SocketAsyncEventArgs buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            int count = this.bufferCount;
            if (count > this.bufferLimit && (count - 1) == Interlocked.CompareExchange(ref this.bufferLimit, count - 1, count))
                buffer.Dispose();
            else
                this.buffers.Push(buffer);
        }

        private readonly ConcurrentStack<SocketAsyncEventArgs> buffers = new ConcurrentStack<SocketAsyncEventArgs>();
        private readonly int defaultBufferSize;
        private int bufferLimit;
        private int bufferCount;
    }
}
