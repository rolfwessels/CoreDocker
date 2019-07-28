using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.Components.Projects;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.GraphQl.DynamicQuery;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Shared.Models.Users;
using CoreDocker.Utilities.Helpers;
using GraphQL.Types;
using Serilog;

namespace CoreDocker.Api.Components.Users
{
    public class UsersQuerySpecification : ObjectGraphType<object>
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public UsersQuerySpecification(IUserLookup users)
        {
            var safe = new Safe(_log);
            var options = Options(users);
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
                "list",
                Description = "all users",
                options.GetArguments(),
                context => options.Query(context,
                    new UserPagedLookupOptions() {Sort = UserPagedLookupOptions.SortOptions.Name})
            ).RequirePermission(Activity.ReadUsers);

            Field<PagedListGraphType<User, UserSpecification>>(
                "paged",
                Description = "all users paged",
                options.GetArguments(),
                context => options.Paged(context)
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
                    new RoleModel {Name = x.Name, Activities = x.Activities.Select(a => a.ToString()).ToList()}))
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

        private static GraphQlQueryOptions<User, UserPagedLookupOptions> Options(IUserLookup users)
        {
            var graphQlQueryOptions = new GraphQlQueryOptions<User, UserPagedLookupOptions>(users.GetPagedUsers)
                .AddArgument(new QueryArgument<StringGraphType>
                {
                    Name = "search",
                    Description = "Search by name,email or id"
                }, (x, c) => x.Search = c.GetArgument<string>("search"))
                .AddArgument(new QueryArgument<StringGraphType>
                {
                    Name = "sort",
                    Description = $"Sort by {EnumHelper.Values<UserPagedLookupOptions.SortOptions>().StringJoin()}"
                }, (x, c) => x.Sort = c.GetArgument<UserPagedLookupOptions.SortOptions>("sort"));


            return graphQlQueryOptions;
        }

        #region Private Methods

        private static async Task<User> Me(Task<User> users)
        {
            var user = await users;
            return user;
        }

        #endregion
    }
}
