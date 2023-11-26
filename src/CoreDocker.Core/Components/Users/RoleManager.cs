using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Core.Components.Users
{
    public class RoleManager : IRoleManager
    {
        public static Role Admin = new("Admin", EnumHelper.ToArray<Activity>().ToList());

        public static Role Guest = new("Guest", EnumHelper.ToArray<Activity>()
            .Where(x => x != Activity.ReadUsers && (x.ToString().StartsWith("Read") || x == Activity.Subscribe))
            .ToList()
        );

        static RoleManager()
        {
            All = new List<Role>
            {
                Admin,
                Guest
            };
        }

        public static List<Role> All { get; }

        public Task<Role?> GetRoleByName(string name)
        {
            return Task.FromResult(GetRole(name));
        }

        public Task<List<Role>> Get()
        {
            return Task.FromResult(All.ToList());
        }

        public static Role? GetRole(string name)
        {
            return All.FirstOrDefault(x => x.Name == name);
        }

        public static bool IsAuthorizedActivity(Activity[] activities, params string[] roleName)
        {
            if (roleName.Contains(Admin.Name))
            {
                return true;
            }

            var allActivities = Activities(roleName).ToArray();
            return activities.All(allActivities.Contains);
        }

        public static bool AreValidRoles(List<string> userRoles)
        {
            var roles = All.Select(x => x.Name).ToArray();
            return userRoles.All(role => roles.Contains(role));
        }

        public static string[] GetRoles(Activity permission)
        {
            return All.Where(x => x.Activities.Contains(permission)).Select(x => x.Name).ToArray();
        }

        private static IEnumerable<Activity> Activities(IEnumerable<string> rolesByName)
        {
            return All.Where(x => rolesByName.Contains(x.Name)).SelectMany(x => x.Activities).ToArray();
        }
    }
}