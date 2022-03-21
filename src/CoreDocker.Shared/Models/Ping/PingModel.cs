namespace CoreDocker.Shared.Models.Ping
{
    public record PingModel(string Environment, string Version, string Database, string MachineName);
}