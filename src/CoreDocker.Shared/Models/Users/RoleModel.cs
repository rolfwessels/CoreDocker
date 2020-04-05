using System.Collections.Generic;

namespace CoreDocker.Shared.Models.Users
{
    public class RoleModel
    {
        public string Name { get; set; }
        public List<string> Activities { get; set; }
    }
}