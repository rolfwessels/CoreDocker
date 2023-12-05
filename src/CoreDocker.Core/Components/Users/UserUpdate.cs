using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Validation;

namespace CoreDocker.Core.Components.Users
{
    public class UserUpdate
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
                    var foundUser = await connection.Users.FindOrThrow(request.Id);
                    var updatedUser = request.ToDao(foundUser);


                    _validation.ValidateAndThrow(updatedUser);
                    await connection.Users.UpdateOne(x => x.Id == foundUser.Id, calls => calls
                        .Set(x => x.Name, updatedUser.Name)
                        .Set(x => x.Email, updatedUser.Email)
                        .Set(x => x.HashedPassword, updatedUser.HashedPassword)
                        .Set(x => x.Roles, updatedUser.Roles)
                    );
                }

                await _commander.Notify(request.ToEvent(), cancellationToken);
            }
        }

        public class Notification : CommandNotificationBase
        {
            public string Name { get; set; } = null!;
            public string Email { get; set; } = null!;
            public bool PasswordChanged { get; set; } = false;
            public List<string> Roles { get; set; } = new();

            public override string EventName => "UserUpdated";
        }

        public record Request(string Id, string Name, string? Password, List<string> Roles, string Email)
            : CommandRequestBase(Id);
    }
}