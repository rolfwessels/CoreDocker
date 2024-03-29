﻿using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Validation;

namespace CoreDocker.Core.Components.Projects
{
    public class ProjectUpdateName
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
                using (var connection = _persistence.GetConnection())
                {
                    var foundProject = await connection.Projects.FindOrThrow(request.Id);
                    var updatedProject = request.ToDao(foundProject);
                    _validation.ValidateAndThrow(updatedProject);
                    await connection.Projects.UpdateOne(x => x.Id == foundProject.Id, calls => calls
                        .Set(x => x.Name, updatedProject.Name)
                    );
                }

                await _commander.Notify(request.ToEvent(), cancellationToken);
            }
        }

        public class Notification : CommandNotificationBase
        {
            public string Name { get; set; } = null!;

            public override string EventName => "ProjectUpdatedName";
        }

        public record Request(string Id, string Name)
            : CommandRequestBase(Id);
    }
}