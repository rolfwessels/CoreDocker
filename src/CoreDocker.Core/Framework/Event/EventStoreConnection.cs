using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Utilities.Serializer;
using EventStore.Client;
using Newtonsoft.Json;
using Serilog;

namespace CoreDocker.Core.Framework.Event
{
    public class EventStoreConnection
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Lazy<EventStoreClient> _client;
        private EventStoreClient Client => _client.Value;
        private Dictionary<string,Type> _types = new Dictionary<string, Type>();
        private readonly IStringify _stringify;

        public EventStoreConnection(string connectionString = "esdb://localhost:2113?Tls=false")
        {
            _stringify = new StringifyJson();
            _client = new Lazy<EventStoreClient>(() =>Init(connectionString)); ;
        }

        private EventStoreClient Init(string connectionString)
        {
            var settings = EventStoreClientSettings.Create(connectionString);
            var client = new EventStoreClient(settings);
            return client;
        }

        public async Task Append<T>(string streamName, T value, CancellationToken cancellationToken)
        {
            var eventData = new EventData(
                Uuid.NewUuid(),
                GetTypeName<T>(),
                _stringify.SerializeToUtf8Bytes(value)
            );
            await Client.AppendToStreamAsync(
                streamName,
                StreamState.Any,
                new[] { eventData },
                cancellationToken: cancellationToken
            );
        }

        
        public async IAsyncEnumerable<EventHolder> Read(string streamName, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var result = Client.ReadStreamAsync(Direction.Forwards, streamName, StreamPosition.Start);
            var events = await result.ToListAsync(cancellationToken);
            foreach (var e in events)
            {
                if (_types.TryGetValue(e.Event.EventType, out var type))
                {
                    var deserialize = _stringify.Deserialize(type, e.Event.Data);
                    _log.Debug($"EventStoreConnection:Read {e.Event.EventType} from {e.Event.Data}");
                    yield return EventHolder.From(e.Event.EventType, deserialize);
                }
                else
                {
                    _log.Error($"Could not find type [{e.Event.EventType}] in registered types [{_types.Values.StringJoin()}] ");
                }
            }
        }

        private string GetTypeName<T>()
        {
            return typeof(T).Name;
        }

        public void Register<T>()
        {
            _types.Add(GetTypeName<T>(), typeof(T));
        }

        public async Task RemoveSteam(string streamName)
        {
            await Client.TombstoneAsync(streamName, StreamState.Any, e => { });
        }
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