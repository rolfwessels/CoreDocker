using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.GraphQl.DynamicQuery;
using CoreDocker.Api.Mappers;
using CoreDocker.Core.Components.Users;
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

        public UsersQuerySpecification(IUserManager users)
        {
            var safe = new Safe(_log);
            var options = new GraphQlQueryOptions<IUserManager, UserModel, User>(users);
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
                resolve: safe.Wrap(context => Me(GraphQlUserContextHelper.User(context)))
            ).RequireAuthorization();


            Field<ListGraphType<RoleSpecification>>(
                "roles",
                Description = "All roles",
                resolve: safe.Wrap(context => RoleManager.All.Select(x =>
                    new RoleModel() {Name = x.Name, Activities = x.Activities.Select(a => a.ToString()).ToList()}))
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
                safe.Wrap(context => RoleManager.GetRole(context.GetArgument<string>("name")))
            );

        }

        #region Private Methods

       
        private static async Task<UserModel> Me(Task<User> users)
        {
            var user = await users;
            return user.ToModel();
        }

        #endregion
    }
}