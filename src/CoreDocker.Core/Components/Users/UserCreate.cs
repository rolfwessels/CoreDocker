using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Validation;

namespace CoreDocker.Core.Components.Users
{
    public class UserCreate
    {
        public class Handler : CommandHandlerBase<Request>
        {
            private readonly ICommander _commander;
            private readonly IGeneralUnitOfWorkFactory _persistance;
            private readonly IValidatorFactory _validation;

            public Handler(IGeneralUnitOfWorkFactory persistance,
                IValidatorFactory validation,
                ICommander commander)
            {
                _persistance = persistance;
                _validation = validation;
                _commander = commander;
            }

            public override async Task ProcessCommand(Request request, CancellationToken cancellationToken)
            {
                var user = request.ToDao();
                using (var connection = _persistance.GetConnection())
                {
                    _validation.ValidateAndThrow(user);
                    user.ValidateRolesAndThrow();
                    await connection.Users.Add(user);
                }

                await _commander.Notify(request.ToEvent(), cancellationToken);
            }
        }

        public class Notification : CommandNotificationBase
        {
            public string Name { get; set; } = null!;
            public string Email { get; set; } = null!;
            public List<string> Roles { get; set; } = null!;

            public override string EventName => "UserCreated";
        }

        public record Request : CommandRequestBase
        {
            public Request(string id, string name, string email, string password, List<string> roles) : base(id)
            {
                if (roles == null || !roles.Any())
                {
                    throw new ArgumentNullException(nameof(roles));
                }

                Name = name ?? throw new ArgumentNullException(nameof(name));
                Email = email ?? throw new ArgumentNullException(nameof(email));
                Password = password;
                Roles = roles;
            }

            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public List<string> Roles { get; set; }
        }
    }
}