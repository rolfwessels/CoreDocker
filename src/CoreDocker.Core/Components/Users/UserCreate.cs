using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Validation;

namespace CoreDocker.Core.Components.Users
{
    public class UserCreate
    {
        #region Nested type: Handler

        public class Handler : CommandHandlerBase<Request>
        {
            private readonly ICommander _commander;
            private readonly IValidatorFactory _validation;
            private readonly IGeneralUnitOfWorkFactory _persistance;

            public Handler(IGeneralUnitOfWorkFactory persistance, IValidatorFactory validation,
                ICommander commander)
            {
                _persistance = persistance;
                _validation = validation;
                _commander = commander;
            }

            #region Overrides of CommandHandlerBase<Request>

            public override async Task ProcessCommand(Request request)
            {
                var user = request.ToDao();
                using (var connection = _persistance.GetConnection())
                {
                    _validation.ValidateAndThrow(user);
                    user.ValidateRolesAndThrow();
                    await connection.Users.Add(user);
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
            public string Email { get; set; }
            public List<string> Roles { get; set; }
        }

        #endregion

        #region Nested type: Request

        public class Request : CommandRequestBase
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public List<string> Roles { get; set; }

            public static Request From(string id, string name, string email, string password, List<string> roles)
            {
                if (id == null) throw new ArgumentNullException(nameof(id));
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (email == null) throw new ArgumentNullException(nameof(email));
                if (roles == null || !roles.Any()) throw new ArgumentNullException(nameof(roles));

                return new Request
                {
                    Id = id,
                    Name = name,
                    Email = email,
                    Password = password,
                    Roles = roles
                };
            }
        }

        #endregion
    }
}