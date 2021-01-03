using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.Event;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Dal.InMemoryCollections;
using CoreDocker.Dal.Models.SystemEvents;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Serializer;
using CoreDocker.Utilities.Tests;
using NUnit.Framework;
using FluentAssertions;

namespace CoreDocker.Core.Tests.Framework.Event
{
    [TestFixture]
    public class EventStoreConnectionTests
    {

        private IEventStoreConnection _store;

        #region Setup/Teardown

        public void Setup()
        {
            TestLoggingHelper.EnsureExists();
            _store = new EventStoreConnection(new FakeRepository<SystemEvent>(), new Messenger(), new StringifyJson());
        }

        #endregion

        [Test]
        public async Task CreateAndConsumeEvents_WhenCalled_ShouldCreateEventAndConsumeIt()
        {
            // arrange
            Setup();
            var expectedBefore = Guid.NewGuid();
            var expectedAfter = Guid.NewGuid();
            _store.Register<SampleCreate>();
            
            
            // action
            var cancellationTokenSource = new CancellationTokenSource();
            await _store.Append( new SampleCreate {Create = expectedBefore }, cancellationTokenSource.Token);
            await _store.Append( new SampleCreate {Create = expectedAfter }, cancellationTokenSource.Token);
            var result = _store.Read( cancellationTokenSource.Token);
            var list = await result
                .OfType<EventHolderTyped<SampleCreate>>()
                .Select(x => x.Typed)
                .ToListAsync(cancellationTokenSource.Token);
            // activity

            list.Select(x=>x.Create).Should().Contain(expectedBefore);
            list.Select(x=>x.Create).Should().Contain(expectedAfter);
        }

        [Test]
        public async Task CreateAndConsumeEvents_WhenCalled_ShouldAlwaysWait()
        {
            // arrange
            Setup();
            var expectedBefore = Guid.NewGuid();
            var expectedAfter = Guid.NewGuid();
            _store.Register<SampleCreate>();


            // action
            var cancellationTokenSource = new CancellationTokenSource();
            await _store.Append(new SampleCreate { Create = expectedBefore }, cancellationTokenSource.Token);
           
            var result = _store.Read(cancellationTokenSource.Token);
            var listAsync = result
                .OfType<EventHolderTyped<SampleCreate>>()
                .Select(x => x.Typed)
                .ToListAsync(cancellationTokenSource.Token);
            await _store.Append(new SampleCreate { Create = expectedAfter }, cancellationTokenSource.Token);
            // activity
            var list = await listAsync;
            list.Select(x => x.Create).Should().Contain(expectedBefore);
            list.Select(x => x.Create).Should().Contain(expectedAfter);
        }


        public class SampleCreate
        {
            public Guid Create { get; set; }
        }
    }

    
}