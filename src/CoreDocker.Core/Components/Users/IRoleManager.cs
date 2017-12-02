using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Dal.Models;

namespace CoreDocker.Core.Components.Users
{
    public interface IRoleManager 
    {
        Task<Role> GetRoleByName(string name);
        Task<List<Role>> Get();
    }
}