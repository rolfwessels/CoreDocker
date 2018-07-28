using System.Reflection;
using CoreDocker.Shared.Models.Users;
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

            Field<UserSpecification>(
                "insert",
                Description = "add a user",
                new QueryArguments(
                    new QueryArgument<UserCreateUpdateSpecification> {Name = Value}
                ), safe.Wrap(context =>
                {
                    var user = context.GetArgument<UserCreateUpdateModel>(Name = Value);
                    return userManager.Insert(user);
                }));

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
                }));

            Field<BooleanGraphType>(
                "delete",
                Description = "permanently remove a user",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> {Name = "id"}
                ), safe.Wrap(context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    return userManager.Delete(id);
                }));
        }



    }
}