using CoreDocker.Shared.Models.Shared;

namespace CoreDocker.Shared.Models.Users
{
    public class UserReferenceModel : BaseReferenceModelWithName
    {
        public string Email { get; set; }
    }
}