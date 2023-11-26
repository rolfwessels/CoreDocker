using System.Threading;
using System.Threading.Tasks;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public interface ICommander
    {
        Task Notify<T>(T notificationRequest, CancellationToken cancellationToken) where T : CommandNotificationBase;

        Task<CommandResult> Execute<T>(T commandRequest, CancellationToken cancellationToken)
            where T : CommandRequestBase;
    }
}