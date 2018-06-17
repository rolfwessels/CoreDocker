using System.Linq;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UsersSpecification : ObjectGraphType<object>
    {
        public UsersSpecification(UserCommonController users)
        {
            Name = "Users";
            Field<UserSpecification>(
                "byId",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "id",
                        Description = "id of the user"
                    }
                ),
                resolve: context => users.GetById(context.GetArgument<string>("id"))
            );
            Field<ListGraphType<UserSpecification>>(
                "all",
                Description = "all users",
                resolve: context => users.Query(queryable => queryable)
            );
            Field<ListGraphType<UserSpecification>>(
                "recent",
                Description = "recent modified users",
                new QueryArguments(
                    new QueryArgument<IntGraphType>
                    {
                        Name = "first",
                        Description = "id of the user"
                    }
                ),
                context => users
                    .Query(queryable =>
                        queryable
                            .OrderByDescending(x => x.UpdateDate)
                            .Take(context.HasArgument("first") ? context.GetArgument<int>("first") : 100)
                    )
            );
        }
    }


}