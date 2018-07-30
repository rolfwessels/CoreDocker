using System.Reflection;
using CoreDocker.Api.GraphQl;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Shared.Models.Users;
using GraphQL.Authorization;
using GraphQL.Types;
using log4net;

namespace CoreDocker.Api.Components.Users
{
    public class UsersMutationSpecification : ObjectGraphType<object>
    {
        private const string Value = "user";
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UsersMutationSpecification(UserCommonController userManager)
        {
            Name = "UsersMutation";

            var safe = new Safe(_log);
            this.RequiresAuthorization();

            Field<UserSpecification>(
                "insert",
                Description = "add a user",
                new QueryArguments(
                    new QueryArgument<UserCreateUpdateSpecification> {Name = Value}
                ), safe.Wrap(context =>
                {
                    var user = context.GetArgument<UserCreateUpdateModel>(Name = Value);
                    return userManager.Insert(user);
                })).RequirePermission(Activity.UpdateUsers);

            Field<UserSpecification>(
                "update",
                Description = "update a user",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "id"},
                    new QueryArgument<UserCreateUpdateSpecification> {Name = Value}
                ), safe.Wrap(context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    var user = context.GetArgument<UserCreateUpdateModel>(Name = Value);
                    return userManager.Update(id, user);
                })).RequirePermission(Activity.UpdateUsers);

            Field<BooleanGraphType>(
                "delete",
                Description = "permanently remove a user",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "id"}
                ), safe.Wrap(context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    return userManager.Delete(id);
                })).RequirePermission(Activity.DeleteUser); 
        }



    }
}