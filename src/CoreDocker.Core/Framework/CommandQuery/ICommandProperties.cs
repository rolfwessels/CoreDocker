using System;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public interface ICommandProperties
    {
        string CorrelationId { get; }
        string Id { get; }
        DateTime CreatedAt { get; }
    }
}