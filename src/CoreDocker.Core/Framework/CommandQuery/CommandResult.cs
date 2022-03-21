using System;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public record CommandResult(string CorrelationId, DateTime CreatedAt, string Id) : ICommandProperties;
}