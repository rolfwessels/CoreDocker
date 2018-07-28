using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.GraphQl.DynamicQuery;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Shared.Models.Users;
using GraphQL.Types;
using log4net;

namespace CoreDocker.Api.Components.Users
{
    public class UsersSpecification : ObjectGraphType<object>
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UsersSpecification(UserCommonController users)
        {
            var safe = new Safe(_log);
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
                resolve: safe.Wrap(context => users.GetById(context.GetArgument<string>("id")))
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
                safe.Wrap(context => users
                    .Query(queryable =>
                        queryable
                            .OrderByDescending(x => x.UpdateDate)
                            .Take(context.HasArgument("first") ? context.GetArgument<int>("first") : 100)
                    ))
            );

            Field<QueryResultSpecification>(
                "query",
                Description = "query the projects projects",
                options.GetArguments(),
                safe.Wrap(context => options.Query(context))
            );

            Field<UserSpecification>(
                "me",
                Description = "Current user",
                resolve: safe.Wrap(context => Me(users))
            );
        }

        #region Private Methods

        private static async Task<UserModel> Me(UserCommonController users)
        {
            return await users.WhoAmI();
        }

        #endregion
    }
}