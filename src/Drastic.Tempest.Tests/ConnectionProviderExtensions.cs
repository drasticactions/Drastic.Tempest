// <copyright file="ConnectionProviderExtensions.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    public static class ConnectionProviderExtensions
    {
        public static Task<IServerConnection> GetNextServerConnection(this IConnectionProvider self)
        {
            var tcs = new TaskCompletionSource<IServerConnection>();

            EventHandler<ConnectionMadeEventArgs> handler = null;
            handler = new EventHandler<ConnectionMadeEventArgs>((sender, e) =>
            {
                self.ConnectionMade -= handler;
                tcs.SetResult(e.Connection);
            });

            self.ConnectionMade += handler;

            return tcs.Task;
        }
    }
}
