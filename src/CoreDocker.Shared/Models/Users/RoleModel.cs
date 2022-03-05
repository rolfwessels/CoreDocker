using System.Collections.Generic;

namespace CoreDocker.Shared.Models.Users
{
    public record RoleModel(string Name, List<string> Activities);
}