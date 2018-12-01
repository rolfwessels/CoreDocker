using CoreDocker.Dal.Models.Base;

namespace CoreDocker.Dal.Models.Users
{
    public class UserReference : BaseReferenceWithName
    {
        public string Email { get; set; }
    }
}