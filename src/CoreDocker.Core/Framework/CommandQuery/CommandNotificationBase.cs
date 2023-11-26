using System;
using MediatR;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public abstract class CommandNotificationBase : ICommandProperties, INotification
    {
        public string CorrelationId { get; set; } = null!;
        public string Id { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public abstract string EventName { get; }
    }
}