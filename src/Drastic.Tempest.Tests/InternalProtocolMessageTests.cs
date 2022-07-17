// <copyright file="InternalProtocolMessageTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

using Drastic.Tempest.InternalProtocol;

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class InternalProtocolMessageTests
    {
        [Test]
        public void PingMessage()
        {
            var msg = new PingMessage();
            msg.Interval = 500;

            var result = msg.AssertLengthMatches();
            Assert.AreEqual(msg.Interval, result.Interval);
        }

        [Test]
        public void ConnectMessage()
        {
            var msg = new ConnectMessage();
            msg.Protocols = new[] { MockProtocol.Instance };
            msg.SignatureHashAlgorithms = new[] { "SHA1" };

            var result = msg.AssertLengthMatches();
            Assert.IsNotNull(result.Protocols);
            Assert.AreEqual(msg.Protocols.Single(), result.Protocols.Single());
            Assert.IsNotNull(result.SignatureHashAlgorithms);
            Assert.AreEqual("SHA1", result.SignatureHashAlgorithms.Single());
        }

        [TestCase(ConnectionResult.Success, null)]
        [TestCase(ConnectionResult.Custom, "ohai")]
        public void DisconnectMessage(ConnectionResult result, string customReason)
        {
            var msg = new DisconnectMessage();
            msg.Reason = ConnectionResult.Success;
            msg.CustomReason = null;

            var mresult = msg.AssertLengthMatches();
            Assert.AreEqual(msg.Reason, mresult.Reason);
            Assert.AreEqual(msg.CustomReason, mresult.CustomReason);
        }
    }
}
