// <copyright file="ConnectionExtensions.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest
{
    /// <summary>
    /// Holds extension methods for <see cref="IConnection"/>
    /// </summary>
    public static class ConnectionExtensions
    {
        /// <summary>
        /// Sends a message to all of the connections.
        /// </summary>
        /// <param name="connections">The connections to send to.</param>
        /// <param name="msg">The message to send.</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="connections"/> is <c>null</c>.</para><para>--or--</para>
        /// <para><paramref name="msg"/> is <c>null</c>.</para>
        /// </exception>
        public static void Send(this IEnumerable<IConnection> connections, Message msg)
        {
            if (connections == null)
                throw new ArgumentNullException("connections");
            if (msg == null)
                throw new ArgumentNullException("msg");

            foreach (IConnection connection in connections)
                connection.SendAsync(msg);
        }

        public static void Send(this IConnection connection, Message msg)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (msg == null)
                throw new ArgumentNullException("msg");

            connection.SendAsync(msg);
        }
    }
}
