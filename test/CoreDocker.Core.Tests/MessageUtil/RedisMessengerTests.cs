using System;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Utilities.Tests;
using CoreDocker.Utilities.Tests.Tools;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.MessageUtil
{
    [TestFixture]
    [Category("Integration")]
    public class RedisMessengerTests
    {
        private RedisMessenger _messenger;

        #region Setup/Teardown

        public void Setup()
        {
            TestLoggingHelper.EnsureExists();
            _messenger = new RedisMessenger("localhost");
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
            _messenger.Clean();
            _messenger.Count().Should().Be(0);
        }

        #endregion
        
        [Test]
        public void Send_Given_Object_ShouldBeReceived()
        {
            // arrange
            Setup();
            var o = new object();
            string received = null;
            _messenger.Register<SampleMessage>(o, m => received = m.Message);
            // action
            _messenger.Send(new SampleMessage("String"));
            // assert
            TestHelper.WaitForValue(() => received).Should().NotBeNull();
        }

        [Test]
        public void Send_GivenObject_ShouldBeReceivedOnOtherListener()
        {
            // arrange
            Setup();
            var o = new object();
            object received = null;
            _messenger.Register(typeof(SampleMessage), o, m => received = m);
            // action
            _messenger.Send(new SampleMessage("String"));
            // assert
            TestHelper.WaitForValue(()=>received).Should().NotBeNull();
        }
        
        [Test]
        public void Send_GivenRegisteredAndThenUnRegister_ShouldNotRelieveMessage()
        {
            // arrange
            Setup();
            var o = new object();
            string received = null;
            _messenger.Register<SampleMessage>(o, m => received = m.Message);
            _messenger.UnRegister<SampleMessage>(o);
            // action
            _messenger.Send(new SampleMessage("String"));
            // assert
            received.Should().BeNull();
        }

        [Test]
        public void Send_GivenRegisteredAndThenUnRegisterAll_ShouldNotRelieveMessage()
        {
            // arrange
            Setup();
            var o = new object();
            string received = null;
            _messenger.Register<SampleMessage>(o, m => received = m.Message);
            _messenger.UnRegister(o);
            // action
            _messenger.Send(new SampleMessage("String"));
            // assert
            received.Should().BeNull();
            _messenger.Count().Should().Be(0);
        }

        #region Nested type: SampleMessage

        public class SampleMessage : IDisposable
        {
            public SampleMessage(string message)
            {
                Message = message;
            }

            public string Message { get; private set; }

            #region IDisposable Members

            #region Implementation of IDisposable

            public void Dispose()
            {
                Message = null;
            }

            #endregion

            #endregion
        }

        #endregion
    }
}