using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Bumbershoot.Utilities.Helpers;
using Bumbershoot.Utilities.Serializer;
using CoreDocker.Core.Framework.Event;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Dal.InMemoryCollections;
using CoreDocker.Dal.Models.SystemEvents;
using CoreDocker.Dal.Tests;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Framework.Event
{
    [TestFixture]
    public class EventStoreConnectionTests
    {
        private IEventStoreConnection _store = null!;

        #region Setup/Teardown

        public void Setup()
        {
            TestLoggingHelper.EnsureExists();
            _store = new EventStoreConnection(new FakeRepository<SystemEvent>(), new Messenger(), new StringifyJson());
        }

        #endregion

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
            var list = new List<SampleCreate>();
            using (_store.ReadAndFollow(cancellationTokenSource.Token)
                       .Subscribe(holder => list.Add((holder as EventHolderTyped<SampleCreate>)!.Typed)))
            {
                await _store.Append(new SampleCreate { Create = expectedAfter }, cancellationTokenSource.Token);
            }

            list.Select(x => x.Create).Should().Contain(expectedBefore);
            list.Select(x => x.Create).Should().Contain(expectedAfter);
        }

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
            await _store.Append(new SampleCreate { Create = expectedBefore }, cancellationTokenSource.Token);
            await _store.Append(new SampleCreate { Create = expectedAfter }, cancellationTokenSource.Token);
            var list = await _store.Read(cancellationTokenSource.Token)
                .OfType<EventHolder, EventHolderTyped<SampleCreate>>()
                .Select(x => x.Typed)
                .ToList(cancellationTokenSource.Token);
            // activity
            list.Select(x => x.Create).Should().Contain(expectedBefore);
            list.Select(x => x.Create).Should().Contain(expectedAfter);
        }


        public class SampleCreate
        {
            public Guid Create { get; set; }
        }
    }
}