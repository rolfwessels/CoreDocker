using System.Collections.Generic;

namespace CoreDocker.Shared.Models.Users
{
    public record UserCreateUpdateModel(string Name, string Email, string Password, List<string> Roles) 
        : RegisterModel(Name, Email, Password);
}