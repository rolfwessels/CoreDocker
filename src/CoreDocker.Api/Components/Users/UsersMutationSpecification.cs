using CoreDocker.Shared.Models;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UsersMutationSpecification : ObjectGraphType<object>
    {
        private const string Value = "user";
        public UsersMutationSpecification(UserCommonController userManager)
        {
            Name = "MarketAppMutation";

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
        }
    }
}