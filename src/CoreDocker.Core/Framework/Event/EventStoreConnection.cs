using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
        private readonly IRepository<SystemEvent> _events;
        private readonly IMessenger _messenger;
        private readonly IStringify _stringify;
        public readonly Dictionary<string,Type> _types = new Dictionary<string, Type>();

        public EventStoreConnection(IRepository<SystemEvent> events, IMessenger messenger, IStringify stringify)
        {
            _events = events;
            _messenger = messenger;
            _stringify = stringify;
        }

        #region Implementation of IEventStoreConnection

        public IAsyncEnumerable<EventHolder> Read(CancellationToken token)
        {
            var keyCollection = _types.Keys;
            var asyncEnumerable = _events.Find(x => keyCollection.Contains(x.TypeName))
                    .ToAsyncEnumerable()
                    .SelectMany(x => x.ToAsyncEnumerable())
                    .Select(ToEventHolder);
            return asyncEnumerable;
        }

        private EventHolder ToEventHolder(SystemEvent x)
        {
            return EventHolder.From(x.EventName, _stringify.Deserialize(_types[x.TypeName], x.Data.AsReadOnlyMemory()));
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
                var asyncEnumerable = _events.Find(x => keyCollection.Contains(x.TypeName)).Result;
                var receiver = new Object();
                _messenger.Register<SystemEvent>(receiver,e => o.OnNext(ToEventHolder(e)) );
                foreach (var holder in asyncEnumerable.Select(ToEventHolder))
                {
                    o.OnNext(holder);
                }

                //o.OnCompleted();

                return () => { _messenger.UnRegister(receiver); };
            });
            // var observableCollection = new ObservableCollection<EventHolder>();
            // Read(token)
            // _messenger.Register<SystemEvent>(observableCollection, e => observableCollection.Add(ToEventHolder(e)));
            // return observableCollection.ToAsyncEnumerable();
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
            await _events.Add(systemEvent);
            await _messenger.Send(systemEvent);
        }

        #endregion
    }
}