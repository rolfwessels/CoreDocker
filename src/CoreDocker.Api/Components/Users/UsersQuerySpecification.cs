using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.GraphQl.DynamicQuery;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Shared.Models.Users;
using GraphQL.Types;
using log4net;

namespace CoreDocker.Api.Components.Users
{
    public class UsersQuerySpecification : ObjectGraphType<object>
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UsersQuerySpecification(UserCommonController users)
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
            ).RequirePermission(Activity.ReadUsers);

            Field<ListGraphType<UserSpecification>>(
                "all",
                Description = "all users",
                resolve: context => users.Query(queryable => queryable)
            ).RequirePermission(Activity.ReadUsers);

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
            ).RequirePermission(Activity.ReadUsers);

            Field<QueryResultSpecification>(
                "query",
                Description = "query the projects projects",
                options.GetArguments(),
                safe.Wrap(context => options.Query(context))
            ).RequirePermission(Activity.ReadUsers);


            Field<UserSpecification>(
                "me",
                Description = "Current user",
                resolve: safe.Wrap(context => Me(users))
            ).RequireAuthorization();


            Field<ListGraphType<RoleSpecification>>(
                "roles",
                Description = "All roles",
                resolve: safe.Wrap(context => users.Roles())
            );

            Field<RoleSpecification>(
                "role",
                Description = "All roles",
                new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "name",
                        Description = "role name"
                    }
                ),
                safe.Wrap(context => LookupRole(users, context.GetArgument<string>("name")))
            );
        }

        #region Private Methods

        private async Task<RoleModel> LookupRole(UserCommonController users, string getArgument)
        {
            var roleModels = await users.Roles();
            return roleModels.FirstOrDefault(x =>
                string.Equals(x.Name, getArgument, StringComparison.InvariantCultureIgnoreCase));
        }

        private static async Task<UserModel> Me(UserCommonController users)
        {
            return await users.WhoAmI();
        }

        #endregion
    }
}