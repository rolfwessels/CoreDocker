using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Validation;

namespace CoreDocker.Core.Components.Projects
{
    public class ProjectCreate
    {
        public class Handler : CommandHandlerBase<Request>
        {
            private readonly ICommander _commander;
            private readonly IGeneralUnitOfWorkFactory _persistence;
            private readonly IValidatorFactory _validation;

            public Handler(IGeneralUnitOfWorkFactory persistence,
                IValidatorFactory validation,
                ICommander commander)
            {
                _persistence = persistence;
                _validation = validation;
                _commander = commander;
            }

            public override async Task ProcessCommand(Request request, CancellationToken cancellationToken)
            {
                var project = request.ToDao();
                using (var connection = _persistence.GetConnection())
                {
                    _validation.ValidateAndThrow(project);
                    await connection.Projects.Add(project);
                }

                await _commander.Notify(request.ToEvent(), cancellationToken);
            }
        }

        public class Notification : CommandNotificationBase
        {
            public string Name { get; set; } = null!;

            public override string EventName => "ProjectCreated";
        }

        public record Request(string Id, string Name)
            : CommandRequestBase(Id);
    }
}