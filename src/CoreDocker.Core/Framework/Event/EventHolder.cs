using System;

namespace CoreDocker.Core.Framework.Event
{
    public class EventHolder
    {
        protected EventHolder(string eventType, object value)
        {
            EventType = eventType;
            Value = value;
        }

        public string EventType { get; }
        public object Value { get; }

        public static EventHolder From(string eventType, object value)
        {
            var generic = typeof(EventHolderTyped<>);
            var constructed = generic.MakeGenericType(value.GetType());
            var eventHolder = Activator.CreateInstance(constructed, eventType, value) as EventHolder;
            return eventHolder ?? throw new InvalidOperationException("Could not create event holder");
        }
    }
}