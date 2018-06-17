using System.Threading.Tasks;
using CoreDocker.Shared.Interfaces.Base;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Projects;

namespace CoreDocker.Shared.Interfaces.Shared
{
    public interface IProjectControllerActions : ICrudController<ProjectModel, ProjectCreateUpdateModel>
	{
	}
}