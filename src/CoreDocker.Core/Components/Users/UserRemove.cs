using System;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Persistence;

namespace CoreDocker.Core.Components.Users
{
    public class UserRemove
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
                    var foundUser = await connection.Users.FindOrThrow(request.Id);
                    var removed = await connection.Users.Remove(x => x.Id == foundUser.Id);
                    await _commander.SendEvent(request.ToEvent(removed));
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
                    Id = id,
                };
            }
        }

        #endregion
    }
}