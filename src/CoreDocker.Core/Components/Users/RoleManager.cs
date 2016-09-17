using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Core.BusinessLogic.Components.Interfaces;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Enums;
using CoreDocker.Utilities.Helpers;

namespace CoreDocker.Core.BusinessLogic.Components
{
    public class RoleManager : IRoleManager
    {
        private static readonly List<Role> _roles;
        public static Role Admin = new Role() { Name = "Admin", Activities = EnumHelper.ToArray<Activity>().ToList()};
        public static Role Guest = new Role()
        {
            Name = "Guest",
            Activities = EnumHelper.ToArray<Activity>()
            .Where(x => (x != Activity.ReadUsers) && ( x.ToString().StartsWith("Read") || x == Activity.Subscribe) )
            .ToList()
        };

        static RoleManager() 
        {
            _roles = new List<Role>
            {
                Admin,
                Guest
            };
        }

        #region IRoleManager Members

        public Task<Role> GetRoleByName(string name)
        {
            return Task.FromResult( GetRole(name));
        }

        public Task<List<Role>> Get()
        {
            return Task.FromResult(_roles.ToList());
        }

        public static Role GetRole(string name)
        {
            return _roles.FirstOrDefault(x => x.Name == name);
        }

        private static IEnumerable<Activity> Activities(IEnumerable<string> rolesByName)
        {
            return _roles.Where(x => rolesByName.Contains(x.Name)).SelectMany(x => x.Activities).ToArray();
        }

        #endregion

        public static bool IsAuthorizedActivity(Activity[] activities, params string[] roleName)
        {
            if (roleName.Contains(Admin.Name)) return true;
            var allActivities = Activities(roleName).ToArray();
            return activities.All(allActivities.Contains);
        }

        
    }
}