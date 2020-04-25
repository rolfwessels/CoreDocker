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
using CoreDocker.Utilities.Helpers;
using HotChocolate.Types;
using Serilog;

namespace CoreDocker.Api.Components.Users
{
    public class UsersQuerySpecification : ObjectType<UsersQuerySpecification.UsersQuery>
    {
        private readonly IUserLookup _userLookup;
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        public UsersQuerySpecification(IUserLookup userLookup)
        {
            _userLookup = userLookup;
        }

        protected override void Configure(IObjectTypeDescriptor<UsersQuery> descriptor)
        {
            var options = Options();
            Name = "Users";

            descriptor.Field("byId")
                .Description("Get user by id")
                .Type<NonNullType<UserSpecification>>()
                .Argument("id", x => x.Description("id of the user").Type<StringType>())
                .Resolver(context => _userLookup.GetById(context.Argument<string>("id")))
                .RequirePermission(Activity.ReadUsers);

            descriptor.Field("paged")
                .Description("all users paged")
                .Type<NonNullType<PagedListGraphType<User, UserSpecification>>>()
                .AddOptions(options)
                .Resolver(x => options.Paged(x))
                .RequirePermission(Activity.ReadUsers);

            descriptor.Field("me")
                .Description("Current user")
                .Type<NonNullType<UserSpecification>>()
                .Resolver(context => Me(context.GetUser()))
                .RequireAuthorization();

            descriptor.Field("roles")
                .Description("All roles")
                .Type<NonNullType<ListType<RoleSpecification>>>()
                .Resolver(context => RoleManager.All.Select(x =>
                    new RoleModel {Name = x.Name, Activities = x.Activities.Select(a => a.ToString()).ToList()}));

            descriptor.Field("role")
                .Description("Get role by name")
                .Type<NonNullType<RoleSpecification>>()
                .Argument("name", x => x.Description("role name").Type<StringType>())
                .Resolver(context => RoleManager.GetRole(context.Argument<string>("name")).ToModel());
        }

        private GraphQlQueryOptions<User, UserPagedLookupOptions> Options()
        {
            var graphQlQueryOptions = new GraphQlQueryOptions<User, UserPagedLookupOptions>(_userLookup.GetPagedUsers)
                .AddArguments<StringType>("search", "Search by name,email or id",
                    (x, c) => x.Search = c.Argument<string>("search"))
                .AddArguments<StringType>("sort",
                    $"Sort by {EnumHelper.Values<UserPagedLookupOptions.SortOptions>().StringJoin()}",
                    (x, c) => x.Sort = c.Argument<UserPagedLookupOptions.SortOptions>("sort"));
            return graphQlQueryOptions;
        }

        #region Private Methods

        private static async Task<User> Me(Task<User> users)
        {
            var user = await users;
            return user.Dump("0000000000User");
        }

        #endregion

        public class UsersQuery
        {
        }
    }
}