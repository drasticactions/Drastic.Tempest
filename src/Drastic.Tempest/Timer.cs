// <copyright file="Timer.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public sealed class Timer
            : IDisposable
    {
        public Timer(int interval)
        {
            if (interval < 0)
            {
                throw new ArgumentOutOfRangeException("interval");
            }

            this.interval = interval;
        }

        /// <summary>
        /// Raised when <see cref="Interval"/> has been reached by the timer.
        /// </summary>
        /// <seealso cref="Interval"/>
        public event EventHandler TimesUp;

        /// <summary>
        /// Gets or sets whether the timer should reset and start timing again automatically.
        /// </summary>
        /// <remarks>Defaults to <c>true</c>.</remarks>
        public bool AutoReset
        {
            get { return this.autoReset; }
            set { this.autoReset = value; }
        }

        /// <summary>
        /// Gets or sets the time interval in milliseconds.
        /// </summary>
        /// <seealso cref="TimesUp"/>
        public int Interval
        {
            get { return this.interval; }
            set { this.interval = value; }
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public void Start()
        {
            if (!this.alive)
            {
                throw new ObjectDisposedException("Timer");
            }

            lock (this.sync)
            {
                if (this.timerThread == null)
                {
                    (this.timerThread = new Thread(TimerProcess)
                    {
                        IsBackground = true,
                        Name = "Timer",
                    }).Start();
                }

                this.running = true;
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            if (!this.alive)
            {
                throw new ObjectDisposedException("Timer");
            }

            this.running = false;
        }

        public void Dispose()
        {
            if (!this.alive)
            {
                return;
            }

            this.Stop();

            this.alive = false;
            this.timerThread = null;
        }

        private int interval;

        private bool autoReset = true;
        private volatile bool alive = true;
        private volatile bool running;

        private readonly object sync = new object();
        private Thread timerThread;

        private void TimerProcess()
        {
            DateTime last = DateTime.Now;

            while (this.alive)
            {
                Thread.Sleep(this.interval / 4);
                if (!this.running)
                {
                    last = DateTime.Now;
                }
                else if (DateTime.Now.Subtract(last).TotalMilliseconds > this.interval)
                {
                    this.TimesUp?.Invoke(this, EventArgs.Empty);

                    last = DateTime.Now;

                    if (!this.autoReset)
                    {
                        this.running = false;
                    }
                }
            }
        }
    }
}
