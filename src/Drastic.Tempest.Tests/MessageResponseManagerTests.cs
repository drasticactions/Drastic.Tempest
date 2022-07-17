// <copyright file="MessageResponseManagerTests.cs" company="Tempest Contributors">
// Copyright (c) Tempest Contributors. All rights reserved.
// </copyright>

namespace Drastic.Tempest.Tests
{
    [TestFixture]
    public class MessageResponseManagerTests
    {
        [Test]
        public void SendFor()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            var mrm = new MessageResponseManager();

            var msg = new BlankMessage { Header = new MessageHeader { MessageId = 1 } };

            Task<Message> response = mrm.SendFor(msg, tcs.Task);

            var responseMsg = new BlankMessage
            {
                Header = new MessageHeader
                {
                    MessageId = 2,
                    ResponseMessageId = 1,
                    IsResponse = true
                }
            };

            mrm.Receive(responseMsg);

            if (!response.Wait(10000))
                Assert.Fail("Task never completed");

            Assert.AreSame(responseMsg, response.Result);
        }

        [Test]
        public void SendForSendFailed()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(false);

            var mrm = new MessageResponseManager();

            var msg = new BlankMessage { Header = new MessageHeader { MessageId = 1 } };

            Task<Message> response = mrm.SendFor(msg, tcs.Task);

            try
            {
                if (!response.Wait(10000))
                    Assert.Fail("Task never completed");

                Assert.Fail("Did not throw cancel exception");
            }
            catch (AggregateException aex)
            {
                Assert.IsTrue(response.IsCanceled);
                Assert.That(aex.Flatten().InnerException, Is.InstanceOf<OperationCanceledException>());
            }
        }

        [Test]
        public void SendForTimeout()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            var mrm = new MessageResponseManager();

            var msg = new BlankMessage { Header = new MessageHeader { MessageId = 1 } };

            Task<Message> response = mrm.SendFor(msg, tcs.Task, 1000);

            DateTime start = DateTime.Now;
            while ((DateTime.Now - start) < TimeSpan.FromSeconds(2))
            {
                mrm.CheckTimeouts();
                Thread.Sleep(1);
            }

            try
            {
                if (!response.Wait(10000))
                    Assert.Fail("Task never completed");

                Assert.Fail("Did not throw cancel exception");
            }
            catch (AggregateException aex)
            {
                Assert.IsTrue(response.IsCanceled);
                Assert.That(aex.Flatten().InnerException, Is.InstanceOf<OperationCanceledException>());
            }
        }

        [Test]
        public void SendForTimeoutZero()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            var mrm = new MessageResponseManager();

            var msg = new BlankMessage { Header = new MessageHeader { MessageId = 1 } };

            Task<Message> response = mrm.SendFor(msg, tcs.Task, 0);

            mrm.CheckTimeouts();

            var responseMsg = new BlankMessage
            {
                Header = new MessageHeader
                {
                    MessageId = 2,
                    ResponseMessageId = 1,
                    IsResponse = true
                }
            };

            mrm.Receive(responseMsg);

            Thread.Sleep(1000);

            if (!response.Wait(10000))
                Assert.Fail("Task never completed");
        }

        [Test]
        public void SendForTimeoutSuccess()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            var mrm = new MessageResponseManager();

            var msg = new BlankMessage { Header = new MessageHeader { MessageId = 1 } };

            Task<Message> response = mrm.SendFor(msg, tcs.Task, 1000);

            var responseMsg = new BlankMessage
            {
                Header = new MessageHeader
                {
                    MessageId = 2,
                    ResponseMessageId = 1,
                    IsResponse = true
                }
            };

            mrm.Receive(responseMsg);

            Thread.Sleep(2000);

            mrm.CheckTimeouts();

            if (!response.Wait(10000))
                Assert.Fail("Task never completed");
        }

        [Test]
        public void SendForCancel()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            var mrm = new MessageResponseManager();

            var msg = new BlankMessage { Header = new MessageHeader { MessageId = 1 } };

            var source = new CancellationTokenSource();

            Task<Message> response = mrm.SendFor(msg, tcs.Task, source.Token);

            source.Cancel();

            try
            {
                if (!response.Wait(10000))
                    Assert.Fail("Task never completed");

                Assert.Fail("Did not throw cancel exception");
            }
            catch (AggregateException aex)
            {
                Assert.IsTrue(response.IsCanceled);
                Assert.That(aex.Flatten().InnerException, Is.InstanceOf<OperationCanceledException>());
            }
        }

        [Test]
        public void SendForCancelSuccess()
        {
            var tcs = new TaskCompletionSource<bool>();
            tcs.SetResult(true);

            var mrm = new MessageResponseManager();

            var msg = new BlankMessage { Header = new MessageHeader { MessageId = 1 } };

            var source = new CancellationTokenSource();

            Task<Message> response = mrm.SendFor(msg, tcs.Task, source.Token);

            var responseMsg = new BlankMessage
            {
                Header = new MessageHeader
                {
                    MessageId = 2,
                    ResponseMessageId = 1,
                    IsResponse = true
                }
            };

            mrm.Receive(responseMsg);

            source.Cancel();

            if (!response.Wait(10000))
                Assert.Fail("Task never completed");
        }
    }
}
