using System;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public interface ICommandProperties
    {
        string CorrelationId { get; set; }
        string Id { get; set; }
        DateTime CreatedAt { get; set; }
    }
}
