// <copyright file="ClientDisconnectedEventArgs.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    public class ClientDisconnectedEventArgs
        : EventArgs
    {
        public ClientDisconnectedEventArgs(bool requested, ConnectionResult reason, string customReason)
        {
            Requested = requested;
            Reason = reason;
            CustomReason = customReason;
        }

        /// <summary>
        /// Gets whether the disconnection was requested by LocalClient.
        /// </summary>
        public bool Requested
        {
            get;
            private set;
        }

        public ConnectionResult Reason
        {
            get;
            private set;
        }

        public string CustomReason
        {
            get;
            private set;
        }
    }
}
