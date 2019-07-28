using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Validation;

namespace CoreDocker.Core.Components.Projects
{
    public class ProjectUpdate
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

            public override async Task ProcessCommand(Request request)
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

                await _commander.SendEvent(request.ToEvent());
            }

            #endregion
        }

        #endregion

        #region Nested type: Notification

        public class Notification : CommandNotificationBase
        {
            public string Name { get; set; }
        }

        #endregion

        #region Nested type: Request

        public class Request : CommandRequestBase
        {
            public string Name { get; set; }

            public static Request From(string id, string name)
            {
                if (id == null) throw new ArgumentNullException(nameof(id));
                if (name == null) throw new ArgumentNullException(nameof(name));
                return new Request
                {
                    Id = id,
                    Name = name,
                };
            }
        }

        #endregion
    }
}
