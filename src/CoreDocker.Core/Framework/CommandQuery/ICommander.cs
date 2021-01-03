using System.Threading.Tasks;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public interface ICommander
    {
        Task Notify<T>(T notificationRequest) where T : CommandNotificationBase;
        Task<CommandResult> Execute<T>(T commandRequest) where T : CommandRequestBase;
    }
}