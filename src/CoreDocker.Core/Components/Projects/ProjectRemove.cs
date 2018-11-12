using System;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Persistence;

namespace CoreDocker.Core.Components.Projects
{
    public class ProjectRemove
    {
        #region Nested type: Handler

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

            #region Overrides of CommandHandlerBase<Request>

            public override async Task ProcessCommand(Request request)
            {
                using (var connection = _persistence.GetConnection())
                {
                    var foundProject = await connection.Projects.FindOrThrow(request.Id);
                    await connection.Projects.Remove(x => x.Id == foundProject.Id);
                    await _commander.SendEvent(request.ToEvent());
                }
            }

            #endregion
        }

        #endregion

        #region Nested type: Notification

        public class Notification : CommandNotificationBase
        {
            public bool WasRemoved { get; set; }
        }

        #endregion

        #region Nested type: Request

        public class Request : CommandRequestBase
        {
            public static Request From(string id)
            {
                if (id == null) throw new ArgumentNullException(nameof(id));

                return new Request
                {
                    Id = id
                };
            }
        }

        #endregion
    }
}