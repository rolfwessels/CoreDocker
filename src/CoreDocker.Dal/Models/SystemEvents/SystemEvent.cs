using System;
using CoreDocker.Dal.Models.Base;

namespace CoreDocker.Dal.Models.SystemEvents
{
    public class SystemEvent : BaseDalModelWithId
    {
        public SystemEvent(string correlationId, DateTime createdAt, string eventId, string eventName,  string typeName, string data)
        {
            CorrelationId = correlationId;
            CreatedAt = createdAt;
            EventName = eventName;
            EventId = eventId;
            TypeName = typeName;
            Data = data;
        }

        public SystemEvent()
        {
        }

        public string CorrelationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EventName { get; set; }
        public string EventId { get; set; }
        public string TypeName { get; set; }
        public string Data { get; set; }


        public static string BuildTypeName<T>(T commandRequest)
        {
            return commandRequest.GetType().FullName;
        }
    }
}