﻿using System;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Core.Framework.Settings;
using CoreDocker.Dal.Tests;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
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
            _messenger = new RedisMessenger(new Settings(new ConfigurationBuilder().AddJsonFilesAndEnvironment().Build()).RedisHost);
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
        public async Task Send_Given_Object_ShouldBeReceived()
        {
            // arrange
            Setup();
            var o = new object();
            string received = null;
            _messenger.Register<SampleMessage>(o, m => received = m.Message);
            // action
            await _messenger.Send(new SampleMessage("String"));
            // assert
            TestHelper.WaitForValue(() => received, 3000).Should().NotBeNull();
        }

        [Test]
        public async Task Send_GivenObject_ShouldBeReceivedOnOtherListener()
        {
            // arrange
            Setup();
            var o = new object();
            object received = null;
            _messenger.Register(typeof(SampleMessage), o, m => received = m);
            // action
            await _messenger.Send(new SampleMessage("String"));
            // assert
            TestHelper.WaitForValue(() => received, 3000).Should().NotBeNull();
        }

        [Test]
        public async Task Send_GivenRegisteredAndThenUnRegister_ShouldNotRelieveMessage()
        {
            // arrange
            Setup();
            var o = new object();
            string received = null;
            _messenger.Register<SampleMessage>(o, m => received = m.Message);
            _messenger.UnRegister<SampleMessage>(o);
            // action
            await _messenger.Send(new SampleMessage("String"));
            // assert
            received.Should().BeNull();
        }

        [Test]
        public async Task Send_GivenRegisteredAndThenUnRegisterAll_ShouldNotRelieveMessage()
        {
            // arrange
            Setup();
            var o = new object();
            string received = null;
            _messenger.Register<SampleMessage>(o, m => received = m.Message);
            _messenger.UnRegister(o);
            // action
            await _messenger.Send(new SampleMessage("String"));
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

            
            #region Implementation of IDisposable

            public void Dispose()
            {
                Message = null;
            }

            #endregion

            
        }

        #endregion
    }
}