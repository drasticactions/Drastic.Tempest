// <copyright file="TargetExtensions.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Sockets;

namespace Drastic.Tempest
{
    public static class TargetExtensions
    {
        public static EndPoint ToEndPoint(this Target self)
        {
            if (self == null)
                throw new ArgumentNullException("self");

            IPAddress ip;
            if (IPAddress.TryParse(self.Hostname, out ip))
                return new IPEndPoint(ip, self.Port);

            return new DnsEndPoint(self.Hostname, self.Port);
        }

        public static async Task<IPEndPoint> ToIPEndPointAsync(this Target self)
        {
            EndPoint endPoint = self.ToEndPoint();
            IPEndPoint ipEndPoint = endPoint as IPEndPoint;
            if (ipEndPoint != null)
                return ipEndPoint;

            var dns = endPoint as DnsEndPoint;
            if (dns == null)
                throw new ArgumentException();

            try
            {
                IPHostEntry entry = await Dns.GetHostEntryAsync(dns.Host).ConfigureAwait(false);
                return new IPEndPoint(entry.AddressList.First(), dns.Port);
            }
            catch (SocketException)
            {
                return null;
            }

        }

        public static Target ToTarget(this EndPoint endPoint)
        {
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");

            DnsEndPoint dns = endPoint as DnsEndPoint;
            if (dns != null)
                return new Target(dns.Host, dns.Port);

            IPEndPoint ip = endPoint as IPEndPoint;
            if (ip != null)
                return new Target(ip.Address.ToString(), ip.Port);

            throw new ArgumentException("Unknown endpoint type", "self");
        }
    }
}