using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Dal.Models.SystemEvents;
using CoreDocker.Dal.Persistence;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Serializer;

namespace CoreDocker.Core.Framework.Event
{
    public class EventStoreConnection : IEventStoreConnection
    {
        private readonly IRepository<SystemEvent> _storedEvents;
        private readonly IMessenger _messenger;
        private readonly IStringify _stringify;
        public readonly Dictionary<string,Type> _types = new Dictionary<string, Type>();

        public EventStoreConnection(IRepository<SystemEvent> storedEvents, IMessenger messenger, IStringify stringify)
        {
            _storedEvents = storedEvents;
            _messenger = messenger;
            _stringify = stringify;
        }

        #region Implementation of IEventStoreConnection

        public IAsyncEnumerable<EventHolder> Read(CancellationToken token)
        {
            var keyCollection = _types.Keys;
            var asyncEnumerable = _storedEvents.Find(x => keyCollection.Contains(x.TypeName))
                .ToAsyncEnumerable()
                .Select(ToEventHolder);
            return asyncEnumerable;
        }

        private EventHolder ToEventHolder(SystemEvent systemEvent)
        {
            return EventHolder.From(systemEvent.EventName, _stringify.Deserialize(_types[systemEvent.TypeName], systemEvent.Data.AsReadOnlyMemory()));
        }


        public void Register<T>()
        {
            _types.Add(SystemEvent.BuildTypeName(default(T)), typeof(T));
        }

        public Task RemoveSteam(string streamName)
        {
            throw new NotImplementedException();
        }

        public IObservable<EventHolder> ReadAndFollow(CancellationToken token)
        {
            return Observable.Create<EventHolder>(o =>
            {
                var keyCollection = _types.Keys;
                var asyncEnumerable = _storedEvents.Find(x => keyCollection.Contains(x.TypeName)).Result;
                var receiver = new object();
                _messenger.Register<SystemEvent>(receiver,e => o.OnNext(ToEventHolder(e)) );
                foreach (var holder in asyncEnumerable.Select(ToEventHolder))
                {
                    o.OnNext(holder);
                }
                return () => { _messenger.UnRegister(receiver); };
            });
        }

        public async Task Append<T>(T value, CancellationToken token)
        {
            var commandNotificationBase = value as CommandNotificationBase;
            var systemEvent = new SystemEvent(
                commandNotificationBase?.CorrelationId ?? null,
                commandNotificationBase?.CreatedAt ?? DateTime.Now,
                commandNotificationBase?.Id ?? null,
                commandNotificationBase?.EventName?? null,
                SystemEvent.BuildTypeName(value),
                _stringify.Serialize(value)
            );
            await _storedEvents.Add(systemEvent);
            await _messenger.Send(systemEvent);
        }

        #endregion
    }
}