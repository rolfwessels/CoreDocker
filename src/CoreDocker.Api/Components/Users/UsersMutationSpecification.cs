using System.Collections.Generic;
using System.Reflection;
using CoreDocker.Api.GraphQl;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Shared.Models.Users;
using GraphQL.Types;
using log4net;

namespace CoreDocker.Api.Components.Users
{
    public class UsersMutationSpecification : ObjectGraphType<object>
    {
        private const string Value = "user";
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UsersMutationSpecification(ICommander commander)
        {
            Name = "UsersMutation";
            var safe = new Safe(_log);

            Field<CommandResultSpecification>(
                "create",
                Description = "add a user",
                new QueryArguments(
                    new QueryArgument<UserCreateUpdateSpecification> {Name = Value}
                ), safe.Wrap(context =>
                {
                    var user = context.GetArgument<UserCreateUpdateModel>(Name = Value);
                    return commander.Execute(UserCreate.Request.From(commander.NewId,user.Name,user.Email,user.Password,user.Roles));
                })).RequirePermission(Activity.UpdateUsers);

            Field<CommandResultSpecification>(
                "register",
                Description = "register a new user",
                new QueryArguments(
                    new QueryArgument<RegisterSpecification> {Name = Value}
                ), safe.Wrap(context =>
                {
                    var user = context.GetArgument<RegisterModel>(Name = Value);
                    return commander.Execute(UserCreate.Request.From(commander.NewId, user.Name, user.Email, user.Password, new List<string>() {RoleManager.Guest.Name}));
                }));

            Field<CommandResultSpecification>(
                "update",
                Description = "update a user",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "id"},
                    new QueryArgument<UserCreateUpdateSpecification> {Name = Value}
                ), safe.Wrap(context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    var user = context.GetArgument<UserCreateUpdateModel>(Name = Value);
                    return commander.Execute(UserUpdate.Request.From(id, user.Name, user.Password, user.Roles, user.Email));
                })).RequirePermission(Activity.UpdateUsers);

            Field<CommandResultSpecification>(
                "remove",
                Description = "permanently remove a user",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "id"}
                ), safe.Wrap(context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    return commander.Execute(UserRemove.Request.From(id));
                })).RequirePermission(Activity.DeleteUser);
        }
    }
}