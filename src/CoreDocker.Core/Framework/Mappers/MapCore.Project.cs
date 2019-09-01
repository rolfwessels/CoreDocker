using AutoMapper;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Dal.Models.Projects;

namespace CoreDocker.Core.Framework.Mappers
{
    public static partial class MapCore
    {
        public static void CreateProjectMap(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Project, ProjectReference>();
            cfg.CreateMap<ProjectCreate.Request, Project>()
                .IgnoreCreateUpdate();
            cfg.CreateMap<ProjectCreate.Request, ProjectCreate.Notification>();

            cfg.CreateMap<ProjectUpdate.Request, Project>()
                .IgnoreCreateUpdate();

            cfg.CreateMap<ProjectUpdate.Request, ProjectUpdate.Notification>();

            cfg.CreateMap<ProjectRemove.Request, ProjectRemove.Notification>()
                .ForMember(x => x.WasRemoved, opt => opt.Ignore());
        }


        public static ProjectReference ToReference(this Project project, ProjectReference projectReference = null)
        {
            return GetInstance().Map(project, projectReference);
        }

        public static Project ToDao(this ProjectCreate.Request project, Project projectReference = null)
        {
            return GetInstance().Map(project, projectReference);
        }

        public static ProjectCreate.Notification ToEvent(this ProjectCreate.Request project,
            ProjectCreate.Notification projectReference = null)
        {
            return GetInstance().Map(project, projectReference);
        }

        public static Project ToDao(this ProjectUpdate.Request project, Project projectReference = null)
        {
            return GetInstance().Map(project, projectReference);
        }

        public static ProjectUpdate.Notification ToEvent(this ProjectUpdate.Request project,
            ProjectUpdate.Notification projectReference = null)
        {
            return GetInstance().Map(project, projectReference);
        }


        public static ProjectRemove.Notification ToEvent(this ProjectRemove.Request project,
            ProjectRemove.Notification projectReference = null)
        {
            var notification = GetInstance().Map(project, projectReference);
            return notification;
        }
    }
}

/* scaffolding [
    {
      "FileName": "MapCore.cs",
      "Indexline": "CreateProjectMap",
      "InsertAbove": false,
      "InsertInline": false,
      "Lines": [
        "CreateProjectMap();"
      ]
    }
] scaffolding */
