using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared;
using CoreDocker.Shared.Interfaces.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Reference;

namespace CoreDocker.Sdk.RestApi.Clients
{
    public class ProjectApiClient : BaseCrudApiClient<ProjectModel, ProjectCreateUpdateModel, ProjectReferenceModel>,
        IProjectControllerActions
    {
        public ProjectApiClient(CoreDockerClient dockerClient)
            : base(dockerClient, RouteHelper.ProjectController)
        {
        }

    }
}
