using System;
using System.Collections.Generic;
using CoreDocker.Shared.Models.Shared;

namespace CoreDocker.Shared.Models.Users
{
    public record UserModel(string Name, string Email, string Image, DateTime? LastLoginDate, List<string> Roles, List<string> Activities) : BaseModel
    {
        public override string ToString()
        {
            return $"Id: {Id}, Email: {Email}, Name: {Name}";
        }
    }
}