using System.Threading.Tasks;
using CoreDocker.Dal.Models.SystemEvents;
using CoreDocker.Dal.Persistence;
using CoreDocker.Utilities.Serializer;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public class CommanderPersist : ICommander
    {
        private readonly ICommander _commander;
        private readonly IRepository<SystemEvent> _repository;
        private readonly IStringify _stringify;

        public CommanderPersist(ICommander commander, IRepository<SystemEvent> repository, IStringify stringify)
        {
            _commander = commander;
            _repository = repository;
            _stringify = stringify;
        }

        #region Implementation of ICommander

        public async Task Notify<T>(T notificationRequest) where T : CommandNotificationBase
        {
            await _repository.Add(new SystemEvent(
                notificationRequest.CorrelationId,
                notificationRequest.CreatedAt,
                notificationRequest.Id,
                notificationRequest.EventName,
                SystemEvent.BuildTypeName(notificationRequest),
                _stringify.Serialize(notificationRequest)
            ));
            await _commander.Notify(notificationRequest);
        }

        public async Task<CommandResult> Execute<T>(T commandRequest) where T : CommandRequestBase
        {
            await _repository.Add(new SystemEvent(
                commandRequest.CorrelationId,
                commandRequest.CreatedAt,
                commandRequest.Id,
                null,
                SystemEvent.BuildTypeName(commandRequest),
                _stringify.Serialize(commandRequest)
            ));
            return await _commander.Execute(commandRequest);
        }

        #endregion
    }
}