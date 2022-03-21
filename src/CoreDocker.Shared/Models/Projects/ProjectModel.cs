using CoreDocker.Shared.Models.Shared;

namespace CoreDocker.Shared.Models.Projects
{
    public record ProjectModel : BaseModel
    {
        public string Name { get; set; } = null!;
    }
}