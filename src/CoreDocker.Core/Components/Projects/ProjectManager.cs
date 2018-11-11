using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Persistance;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Core.Components.Projects
{
    public class ProjectManager : BaseManager<Project>, IProjectManager
    {
        public ProjectManager(BaseManagerArguments baseManagerArguments, ILogger<ProjectManager> logger) : base(
            baseManagerArguments, logger)
        {
        }

        #region Overrides of BaseManager<Project>

        protected override IRepository<Project> Repository => _generalUnitOfWork.Projects;

        #endregion
    }
}

/* scaffolding [
    {
      "FileName": "IocCoreBase.cs",
      "Indexline": "As<IUserManager>",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "builder.RegisterType<ProjectManager>().As<IProjectManager>();"
      ]
    },
    {
      "FileName": "MongoGeneralUnitOfWork.cs",
      "Indexline": "Projects = ",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "Projects = new MongoRepository<Project>(database);"
      ]
    },
    {
      "FileName": "IGeneralUnitOfWork.cs",
      "Indexline": "Projects",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "IRepository<Project> Projects { get; }"
      ]
    },
    {
      "FileName": "MongoGeneralUnitOfWork.cs",
      "Indexline": "Projects { get; private set; }",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "public IRepository<Project> Projects { get; private set; }"
      ]
    },
  {
      "FileName": "InMemoryGeneralUnitOfWork.cs",
      "Indexline": "Projects =",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "Projects = new FakeRepository<Project>();"
      ]
    },
    {
      "FileName": "InMemoryGeneralUnitOfWork.cs",
      "Indexline": "Projects { get",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "public IRepository<Project> Projects { get; private set; }"
      ]
    }
] scaffolding */