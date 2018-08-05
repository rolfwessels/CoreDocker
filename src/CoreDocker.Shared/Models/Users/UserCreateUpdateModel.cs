using System.Collections.Generic;

namespace CoreDocker.Shared.Models.Users
{
    public class UserCreateUpdateModel : RegisterModel
    {
        public List<string> Roles { get; set; }
    }
}