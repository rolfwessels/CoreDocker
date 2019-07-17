using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Shared.Models.Users;
using GraphQL.Types;
using log4net;

namespace CoreDocker.Api.Components.Users
{
    public class UserSpecification : ObjectGraphType<User>
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public UserSpecification()
        {
            Name = "User";
            Field(d => d.Id).Description("The id of the user.");
            Field(d => d.Name).Description("The name of the user.");
            Field(d => d.Email).Description("The email of the user.");
            Field(d => d.Roles).Description("The roles of the user.");
            Field<ListGraphType<StringGraphType>>("activities", resolve: context => Roles(context.Source?.Roles),
                description: "The activities that this user is authorized for.");
            Field(d => d.UpdateDate, true, typeof(DateTimeGraphType))
                .Description("The date when the user was last updated.");
            Field(d => d.CreateDate, type: typeof(DateTimeGraphType))
                .Description("The date when the user was created.");
        }

        #region Private Methods

        private static List<string> Roles(List<string> sourceRoles)
        {
            var roles = sourceRoles.Select(RoleManager.GetRole)
                .Where(x => x != null)
                .SelectMany(x => x.Activities)
                .Distinct()
                .Select(x => x.ToString())
                .OrderBy(x => x)
                .ToList();
            return roles;
        }

        #endregion
    }
}