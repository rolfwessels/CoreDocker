using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Shared.Models.Users;

namespace CoreDocker.Api.Components.Users
{
    public class UsersMutation
    {
        private readonly ICommander _commander;

        public UsersMutation(ICommander commander)
        {
            _commander = commander;
        }

        public Task<CommandResult> Create(UserCreateUpdateModel user)
        {
            return _commander.Execute(UserCreate.Request.From(_commander.NewId, user.Name, user.Email,
                user.Password, user.Roles));
        }

        public Task<CommandResult> Update(string id, UserCreateUpdateModel user)
        {
            return _commander.Execute(UserUpdate.Request.From(id, user.Name, user.Password, user.Roles,
                user.Email));
        }

        public Task<CommandResult> Remove(string id)
        {
            return _commander.Execute(UserRemove.Request.From(id));
        }

        public Task<CommandResult> Register(RegisterModel user)
        {
            return _commander.Execute(UserCreate.Request.From(_commander.NewId, user.Name, user.Email,
                user.Password, new List<string>() {RoleManager.Guest.Name}));
        }
    }
}