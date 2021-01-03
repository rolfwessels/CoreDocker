using System;
using System.Collections.Generic;
using System.Linq;
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

        public IAsyncEnumerable<EventHolder> Read(CancellationToken cancellationToken)
        {
            var keyCollection = _types.Keys;
            return _events.Find(x => keyCollection.Contains(x.TypeName))
                .ToAsyncEnumerable()
                .SelectMany(x=>x.ToAsyncEnumerable())
                .Select(x=> EventHolder.From(x.EventName,_stringify.Deserialize(_types[x.TypeName],x.Data.AsReadOnlyMemory())));
        }

        
        public void Register<T>()
        {
            _types.Add(SystemEvent.BuildTypeName(default(T)), typeof(T));
        }

        public Task RemoveSteam(string streamName)
        {
            throw new NotImplementedException();
        }

        public async Task Append<T>(T value, CancellationToken cancellationToken)
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

    public interface IEventStoreConnection
    {
        Task Append<T>(T value, CancellationToken cancellationToken);
        IAsyncEnumerable<EventHolder> Read(CancellationToken cancellationToken);
        void Register<T>();
        Task RemoveSteam(string streamName);
    }

    public class EventHolder
    {
        public string EventType { get; }
        public object Value { get; }

        protected EventHolder(string eventType, object value)
        {
            EventType = eventType;
            Value = value;
        }

        public static EventHolder From(string eventType, object value)
        {
            Type generic = typeof(EventHolderTyped<>);
            Type constructed = generic.MakeGenericType(value.GetType());
            return Activator.CreateInstance(constructed, eventType, value) as EventHolder;
        }
    }

    public class EventHolderTyped<T>: EventHolder
    {
        public EventHolderTyped(string eventType, object value) : base(eventType, value)
        {
        }

        public T Typed => (T) Value;

        
    }
}