using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.GraphQl;
using CoreDocker.Core.Components.Users;
using CoreDocker.Shared.Models.Users;
using GraphQL.Types;
using log4net;
using log4net.Repository;

namespace CoreDocker.Api.Components.Users
{
    public class UserSpecification : ObjectGraphType<UserModel>
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public UserSpecification(UserCommonController user)
        {
            var safe = new Safe(_log);
            Name = "User";
            Field(d => d.Id).Description("The id of the user.");
            Field(d => d.Name).Description("The name of the user.");
            Field(d => d.Email).Description("The email of the user.");
            Field(d => d.Roles).Description("The roles of the user.");
            Field<ListGraphType<StringGraphType>>("activities", resolve: context => Roles(user, context.Source?.Roles));
//                .Description("The activities that this user is authorized for.");
            Field(d => d.UpdateDate,true,typeof(DateTimeGraphType)).Description("The date when the user was last updated.");
            Field(d => d.CreateDate, type: typeof(DateTimeGraphType)).Description("The date when the user was created.");
        }

        private static async Task<List<string>> Roles(UserCommonController user, List<string> sourceRoles)
        {
            
            var roles = await user.Roles();
            return roles.Where(x=> sourceRoles.Contains(x.Name) )
                .SelectMany(x=>x.Activities)
                .Distinct()
                .OrderBy(x=>x)
                .ToList();
        }
    }
}