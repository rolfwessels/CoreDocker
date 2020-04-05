using System.Threading.Tasks;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public interface ICommander
    {
        string NewId { get; }
        Task SendEvent<T>(T @event) where T : CommandNotificationBase;
        Task<CommandResult> Execute<T>(T from) where T : CommandRequestBase;
    }
}