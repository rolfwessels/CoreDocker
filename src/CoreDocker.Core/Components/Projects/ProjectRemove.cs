using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Persistence;

namespace CoreDocker.Core.Components.Projects
{
    public class ProjectRemove
    {
        public class Handler : CommandHandlerBase<Request>
        {
            private readonly ICommander _commander;
            private readonly IGeneralUnitOfWorkFactory _persistence;

            public Handler(IGeneralUnitOfWorkFactory persistence,
                ICommander commander)
            {
                _persistence = persistence;
                _commander = commander;
            }

            public override async Task ProcessCommand(Request request, CancellationToken cancellationToken)
            {
                using var connection = _persistence.GetConnection();
                var foundProject = await connection.Projects.FindOrThrow(request.Id);
                await connection.Projects.Remove(x => x.Id == foundProject.Id);
                await _commander.Notify(request.ToEvent(), cancellationToken);
            }
        }

        public class Notification : CommandNotificationBase
        {
            public override string EventName => "ProjectRemoved";

            public bool WasRemoved { get; set; }
        }

        public record Request(string Id) : CommandRequestBase(Id);
    }
}