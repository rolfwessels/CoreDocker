using System;
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
        #region Nested type: Handler

        public class Handler : CommandHandlerBase<Request>
        {
            private readonly ICommander _commander;
            private readonly IValidatorFactory _validation;
            private readonly IGeneralUnitOfWorkFactory _persistence;

            public Handler(IGeneralUnitOfWorkFactory persistence, IValidatorFactory validation,
                ICommander commander)
            {
                _persistence = persistence;
                _validation = validation;
                _commander = commander;
            }

            #region Overrides of CommandHandlerBase<Request>

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

            #endregion
        }

        #endregion

        #region Nested type: Notification

        public class Notification : CommandNotificationBase
        {
            public string Name { get; set; } = null!;

            #region Overrides of CommandNotificationBase

            public override string EventName => "ProjectCreated";

            #endregion
        }

        #endregion

        #region Nested type: Request

        public record Request(string Id, string Name)
            : CommandRequestBase(Id);

        #endregion
    }
}