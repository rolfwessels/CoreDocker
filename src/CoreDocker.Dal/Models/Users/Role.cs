using System.Collections.Generic;
using CoreDocker.Dal.Models.Auth;

namespace CoreDocker.Dal.Models.Users
{
    public record Role(string Name, List<Activity> Activities);
}