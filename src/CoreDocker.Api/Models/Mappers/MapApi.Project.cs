using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CoreDocker.Shared.Models;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Reference;
using CoreDocker.Shared.Models.Projects;

namespace CoreDocker.Api.Models.Mappers
{
    public static partial class MapApi
    {
        public static Project ToDal(this ProjectCreateUpdateModel model, Project project = null)
        {
            return GetInstance().Map(model, project);
        }

        public static ProjectModel ToModel(this Project project, ProjectModel model = null)
        {
            return GetInstance().Map(project, model);
        }

        public static IEnumerable<ProjectReferenceModel> ToReferenceModelList(this IEnumerable<Project> projects)
        {
            return GetInstance().Map<IEnumerable<Project>, IEnumerable<ProjectReferenceModel>>(projects);
        }

        public static IEnumerable<ProjectModel> ToModelList(this IEnumerable<Project> projects)
        {
            return GetInstance().Map<IEnumerable<Project>, IEnumerable<ProjectModel>>(projects);
        }

        #region Private Methods

        private static void MapProjectModel(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Project, ProjectModel>();
            configuration.CreateMap<Project, ProjectReferenceModel>();
            configuration.CreateMap<ProjectReference, ProjectReferenceModel>();
            configuration.CreateMap<ProjectCreateUpdateModel, Project>().MapToDal();
        }

        #endregion
    }
}

/* scaffolding [
    {
      "FileName": "MapApi.cs",
      "Indexline": "MapProjectModel",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "MapProjectModel();"
      ]
    }
] scaffolding */