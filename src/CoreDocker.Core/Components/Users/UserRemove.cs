using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Persistence;

namespace CoreDocker.Core.Components.Users
{
    public class UserRemove
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
                using (var connection = _persistence.GetConnection())
                {
                    var foundUser = await connection.Users.FindOrThrow(request.Id);
                    var removed = await connection.Users.Remove(x => x.Id == foundUser.Id);
                    await _commander.Notify(request.ToEvent(removed), cancellationToken);
                }
            }
        }

        public class Notification : CommandNotificationBase
        {
            public bool WasRemoved { get; set; }

            public override string EventName => "UserRemoved";
        }

        public record Request(string Id) : CommandRequestBase(Id);
    }
}