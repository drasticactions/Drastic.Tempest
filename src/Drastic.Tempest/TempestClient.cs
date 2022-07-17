﻿// <copyright file="TempestClient.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Collections.Concurrent;
using System.ComponentModel;

namespace Drastic.Tempest
{
    /// <summary>
    /// Tempest clients.
    /// </summary>
    public class TempestClient
        : MessageHandler, IClientContext, INotifyPropertyChanged
    {
        public TempestClient(IClientConnection connection, MessageTypes mtypes)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (!Enum.IsDefined(typeof(MessageTypes), mtypes))
                throw new ArgumentException("Not a valid MessageTypes value", "mtypes");

            this.messageTypes = mtypes;

            this.connection = connection;
            this.connection.Connected += OnConnectionConnected;
            this.connection.Disconnected += OnConnectionDisconnected;

            this.mqueue = new ConcurrentQueue<MessageEventArgs>();
            this.connection.MessageReceived += ConnectionOnMessageReceived;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raised when the client connects.
        /// </summary>
        public event EventHandler<ClientConnectionEventArgs> Connected;

        /// <summary>
        /// Raised with the client is disconnected.
        /// </summary>
        public event EventHandler<ClientDisconnectedEventArgs> Disconnected;

        public IClientConnection Connection
        {
            get { return this.connection; }
        }

        /// <summary>
        /// Gets whether the client is currently connected or not.
        /// </summary>
        public virtual bool IsConnected
        {
            get { return this.connection.IsConnected; }
        }

        /// <summary>
        /// Attempts to asynchronously connect to <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The target to connect to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is <c>null</c>.</exception>
        public virtual Task<ClientConnectionResult> ConnectAsync(Target target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            return this.connection.ConnectAsync(target, this.messageTypes);
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public virtual Task DisconnectAsync()
        {
            return DisconnectAsyncCore(ConnectionResult.Custom, "Requested");
        }

        public virtual Task DisconnectAsync(ConnectionResult reason, string customReason = null)
        {
            return DisconnectAsyncCore(reason, customReason);
        }

        private readonly IClientConnection connection;
        private readonly ConcurrentQueue<MessageEventArgs> mqueue;

        private Task messageRunnerTask;
        private volatile bool running;
        private readonly MessageTypes messageTypes;

        private Task DisconnectAsyncCore(ConnectionResult reason, string customReason)
        {
            Task task = this.connection.DisconnectAsync(reason, customReason);

            this.running = false;

            Task runner = Interlocked.Exchange(ref this.messageRunnerTask, null);
            ((AutoResetEvent)runner?.AsyncState)?.Set();

            MessageEventArgs e;
            while (this.mqueue.TryDequeue(out e))
            {
            }

            if (runner != null)
                task = Task.WhenAll(task, runner);

            return task;
        }

        private void ConnectionOnMessageReceived(object sender, MessageEventArgs e)
        {
            this.mqueue.Enqueue(e);

            Task runner = this.messageRunnerTask;
            ((AutoResetEvent)runner?.AsyncState)?.Set();
        }

        private void MessageRunner(object state)
        {
            AutoResetEvent wait = (AutoResetEvent)state;
            ConcurrentQueue<MessageEventArgs> q = this.mqueue;

            while (this.running)
            {
                while (q.Count > 0 && this.running)
                {
                    MessageEventArgs e;
                    if (!q.TryDequeue(out e))
                        continue;

                    List<Action<MessageEventArgs>> mhandlers = GetHandlers(e.Message);
                    if (mhandlers == null)
                        continue;

                    for (int i = 0; i < mhandlers.Count; ++i)
                        mhandlers[i](e);
                }

                if (q.Count == 0)
                    wait.WaitOne();
            }
        }

        private void OnConnectionDisconnected(object sender, DisconnectedEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("IsConnected"));
            DisconnectAsyncCore(e.Result, e.CustomReason);
            OnDisconnected(new ClientDisconnectedEventArgs(e.Result == ConnectionResult.Custom, e.Result, e.CustomReason));
        }

        private void OnConnectionConnected(object sender, ClientConnectionEventArgs e)
        {
            OnPropertyChanged(new PropertyChangedEventArgs("IsConnected"));

            Task oldRunner = Interlocked.Exchange(ref this.messageRunnerTask, null);
            if (oldRunner != null)
            {
                ((AutoResetEvent)oldRunner.AsyncState).Set();
                oldRunner.Wait();
            }

            this.running = true;
            Task newRunner = Task.Factory.StartNew(MessageRunner, new AutoResetEvent(false), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            Interlocked.Exchange(ref this.messageRunnerTask, newRunner);

            OnConnected(e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var changed = PropertyChanged;
            if (changed != null)
                changed(this, e);
        }

        protected virtual void OnConnected(ClientConnectionEventArgs e)
        {
            EventHandler<ClientConnectionEventArgs> handler = Connected;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnDisconnected(ClientDisconnectedEventArgs e)
        {
            var handler = Disconnected;
            if (handler != null)
                handler(this, e);
        }
    }
}
