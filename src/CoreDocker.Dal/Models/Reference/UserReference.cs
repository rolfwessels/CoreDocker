using CoreDocker.Dal.Models.Base;

namespace CoreDocker.Dal.Models.Reference
{
    public class UserReference : BaseReferenceWithName
    {
        public string Email { get; set; }
    }
}