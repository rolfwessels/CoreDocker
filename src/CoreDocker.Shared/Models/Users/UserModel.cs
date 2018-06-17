using System;
using System.Collections.Generic;
using CoreDocker.Shared.Models.Shared;

namespace CoreDocker.Shared.Models.Users
{
    public class UserModel : BaseModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public List<string> Roles { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Email: {1}, Name: {2}", Id, Email, Name);
        }
    }
}