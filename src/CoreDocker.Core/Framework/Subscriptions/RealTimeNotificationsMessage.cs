namespace CoreDocker.Core.Framework.Subscriptions
{
    public record RealTimeNotificationsMessage(string Id, string Event, string CorrelationId, string? Exception);
}