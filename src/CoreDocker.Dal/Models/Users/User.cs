using System;
using System.Collections.Generic;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Models.Projects;

namespace CoreDocker.Dal.Models.Users
{
    public class User : BaseDalModelWithId
    {
        public User()
        {
            Roles = new List<string>();
        }

        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? HashedPassword { get; set; }
        public List<string> Roles { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public ProjectReference? DefaultProject { get; set; }

        public override string ToString()
        {
            return $"Email: {Email}, Name: {Name}";
        }
    }
}