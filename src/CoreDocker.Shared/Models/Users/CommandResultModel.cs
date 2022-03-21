using System;

namespace CoreDocker.Shared.Models.Users
{
    public record CommandResultModel(string CorrelationId, string Id, DateTime CreatedAt)
    {
    }
}