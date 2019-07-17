using System;
using MediatR;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public class CommandNotificationBase : ICommandProperties, INotification
    {
        #region Implementation of ICommandProperties

        public string CorrelationId { get; set; }
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }

        #endregion
    }
}