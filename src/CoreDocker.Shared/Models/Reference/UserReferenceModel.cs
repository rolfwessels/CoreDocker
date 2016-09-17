using CoreDocker.Shared.Models.Base;

namespace CoreDocker.Shared.Models.Reference
{
    public class UserReferenceModel : BaseReferenceModelWithName
    {
        public string Email { get; set; }
    }
}