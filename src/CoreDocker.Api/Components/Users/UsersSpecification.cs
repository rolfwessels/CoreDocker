using System.Linq;
using CoreDocker.Api.Components.Projects;
using CoreDocker.Dal.Models;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Users;
using GraphQL.Types;

namespace CoreDocker.Api.Components.Users
{
    public class UsersSpecification : ObjectGraphType<object>
    {
        public UsersSpecification(UserCommonController users)
        {
            var options = new GraphQlQueryOptions<UserCommonController, UserModel, User>(users);
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
            Field<QueryResultSpecification>(
                "query",
                Description = "query the projects projects",
                options.GetArguments(),
                context => options.Query(context)
            );
        }
    }


}