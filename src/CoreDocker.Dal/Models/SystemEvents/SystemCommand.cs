using System;
using CoreDocker.Dal.Models.Base;

namespace CoreDocker.Dal.Models.SystemEvents
{
    public class SystemCommand : BaseDalModelWithId
    {
        public SystemCommand(string correlationId, DateTime createdAt, string eventId, string typeName, string data)
        {
            CorrelationId = correlationId;
            CreatedAt = createdAt;
            EventId = eventId;
            TypeName = typeName;
            Data = data;
        }

        public SystemCommand()
        {
        }

        public string CorrelationId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string EventId { get; set; }
        public string TypeName { get; set; }
        public string Data { get; set; }


        public static string BuildTypeName<T>(T commandRequest)
        {
            return commandRequest.GetType().FullName;
        }
    }
}