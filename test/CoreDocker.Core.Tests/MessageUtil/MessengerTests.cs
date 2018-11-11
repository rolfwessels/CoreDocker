﻿using System;
using CoreDocker.Core.Framework.MessageUtil;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.MessageUtil
{
    [TestFixture]
    public class MessengerTests
    {
        private Messenger _messenger;

        #region Setup/Teardown

        public void Setup()
        {
            _messenger = new Messenger();
        }

        [TearDown]
        public void TearDown()
        {
        }

        #endregion

        [Test]
        public void Constructor_WhenCalled_ShouldNotBeNull()
        {
            // arrange
            Setup();
            // assert
            _messenger.Should().NotBeNull();
        }

        [Test]
        public void Send_Given_Object_ShouldBeRecieved()
        {
            // arrange
            Setup();
            var o = new object();
            string recieved = null;
            _messenger.Register<SampleMessage>(o, m => recieved = m.Message);
            // action
            _messenger.Send(new SampleMessage("String"));
            // assert
            recieved.Should().NotBeNull();
        }

        [Test]
        public void Send_GivenObject_ShouldBeRecievedOnOtherListner()
        {
            // arrange
            Setup();
            var o = new object();
            object recieved = null;
            _messenger.Register(typeof(SampleMessage), o, m => recieved = m);
            // action
            _messenger.Send(new SampleMessage("String"));
            // assert
            recieved.Should().NotBeNull();
        }

        [Test]
        public void Send_GivenRegisteredAndThenUnregister_ShouldNotRelieveMessage()
        {
            // arrange
            Setup();
            var o = new object();
            string recieved = null;
            _messenger.Register<SampleMessage>(o, m => recieved = m.Message);
            _messenger.UnRegister<SampleMessage>(o);
            // action
            _messenger.Send(new SampleMessage("String"));
            // assert
            recieved.Should().BeNull();
        }

        [Test]
        public void Send_GivenRegisteredAndThenUnregisterAll_ShouldNotRelieveMessage()
        {
            // arrange
            Setup();
            var o = new object();
            string recieved = null;
            _messenger.Register<SampleMessage>(o, m => recieved = m.Message);
            _messenger.UnRegister(o);
            // action
            _messenger.Send(new SampleMessage("String"));
            // assert
            recieved.Should().BeNull();
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