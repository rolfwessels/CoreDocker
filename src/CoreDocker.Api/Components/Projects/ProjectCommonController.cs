using CoreDocker.Api.WebApi.Controllers;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Shared.Interfaces.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Projects;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectCommonController : BaseCommonController<Project, ProjectModel, ProjectReferenceModel, ProjectCreateUpdateModel>, IProjectControllerActions
    {
        
        public ProjectCommonController(IProjectManager projectManager)
            : base(projectManager)
        {
        }


       
    }
}
/* scaffolding [
    {
      "FileName": "IocApi.cs",
      "Indexline": "RegisterType<ProjectCommonController>",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "builder.RegisterType<ProjectCommonController>();"
      ]
    }
] scaffolding */