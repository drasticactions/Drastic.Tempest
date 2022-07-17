// <copyright file="TargetTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using System.Net.Sockets;
using System.Net;

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class TargetTests
    {
        [Test]
        public void CtorNull()
        {
            Assert.Throws<ArgumentNullException>(() => new Target(null, 0));
        }

        [Test]
        public void Ctor()
        {
            var target = new Target("host", 1234);
            Assert.That(target.Hostname, Is.EqualTo("host"));
            Assert.That(target.Port, Is.EqualTo(1234));
        }

        // Extensions

        [Test]
        public void ToEndPointNull()
        {
            Assert.That(() => TargetExtensions.ToEndPoint(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void ToEndPointIPv4()
        {
            var target = new Target("127.0.0.1", 1234);
            EndPoint endPoint = target.ToEndPoint();

            Assert.That(endPoint, Is.InstanceOf<IPEndPoint>());

            IPEndPoint ip = (IPEndPoint)endPoint;
            Assert.That(ip.Address, Is.EqualTo(IPAddress.Parse("127.0.0.1")));
            Assert.That(ip.AddressFamily, Is.EqualTo(AddressFamily.InterNetwork));
        }

        [Test]
        public void ToEndPointIPv6()
        {
            var target = new Target("::1", 1234);
            EndPoint endPoint = target.ToEndPoint();

            Assert.That(endPoint, Is.InstanceOf<IPEndPoint>());

            IPEndPoint ip = (IPEndPoint)endPoint;
            Assert.That(ip.Address, Is.EqualTo(IPAddress.Parse("::1")));
            Assert.That(ip.AddressFamily, Is.EqualTo(AddressFamily.InterNetworkV6));
            Assert.That(ip.Port, Is.EqualTo(target.Port));
        }

        [Test]
        public void ToEndPointHostname()
        {
            var target = new Target("localhost", 1234);
            EndPoint endPoint = target.ToEndPoint();

            Assert.That(endPoint, Is.InstanceOf<DnsEndPoint>());

            DnsEndPoint dns = (DnsEndPoint)endPoint;
            Assert.That(dns.Host, Is.EqualTo(target.Hostname));
            Assert.That(dns.Port, Is.EqualTo(target.Port));
        }
    }
}
