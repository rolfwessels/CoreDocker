using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Users;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UsersMutationSpecification : ObjectGraphType<object>
    {
        private const string Value = "user";
        public UsersMutationSpecification(UserCommonController userManager)
        {
            Name = "UsersMutation";

            Field<UserSpecification>(
                "insert",
                Description = "add a user",
                new QueryArguments(
                    new QueryArgument<UserCreateUpdateSpecification> { Name = Value }
                ),
                context =>
                {
                    var user = context.GetArgument<UserCreateUpdateModel>(Name = Value);
                    return userManager.Insert(user);
                });

            Field<UserSpecification>(
                "update",
                Description = "update a user",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" },
                    new QueryArgument<UserCreateUpdateSpecification> { Name = Value }
                ),
                context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    var user = context.GetArgument<UserCreateUpdateModel>(Name = Value);
                    return userManager.Update(id, user);
                });

            Field<BooleanGraphType>(
                "delete",
                Description = "permanently remove a user",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id" }
                ),
                context =>
                {
                    var id = context.GetArgument<string>(Name = "id");
                    return userManager.Delete(id);
                });
        }
    }
}