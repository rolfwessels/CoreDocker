using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.Event;
using CoreDocker.Core.Framework.Logging;
using CoreDocker.Core.Tests.Helpers;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Tests;
using CoreDocker.Utilities.Tests.Helpers;
using EventStore.Client;
using NUnit.Framework;
using FluentAssertions;

namespace CoreDocker.Core.Tests.Framework.Event
{
    [TestFixture]
    public class EventStoreConnectionTests
    {

        private EventStoreConnection _store;

        #region Setup/Teardown

        public void Setup()
        {
            TestLoggingHelper.EnsureExists();
            _store = new EventStoreConnection();
        }

        #endregion

        [Test]
        public async Task CreateAndConsumeEvents_WhenCalled_ShouldCreateEventAndConsumeIt()
        {
            // arrange
            Setup();
            var expected = new Random().Next(1000,4000);
            _store.Register<SampleCreate>();
            
            var streamName = "unit-test-"+Guid.NewGuid();
            // action
            var cancellationTokenSource = new CancellationTokenSource();
            await _store.Append(streamName, new SampleCreate {Create = expected}, cancellationTokenSource.Token);
            var result = _store.Read(streamName, cancellationTokenSource.Token);
            var list = await result
                .OfType<EventHolderTyped<SampleCreate>>()
                .Select(x => x.Typed)
                .ToListAsync(cancellationTokenSource.Token);
            // activity
            cancellationTokenSource.Cancel();
            _store.RemoveSteam(streamName).ContinueWithAndLogError();
            list.Select(x=>x.Create).Should().Contain(expected);
            
        }


        public class SampleCreate
        {
            public int Create { get; set; }
        }
    }

   
}