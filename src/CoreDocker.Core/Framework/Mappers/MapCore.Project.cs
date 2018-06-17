using AutoMapper;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Projects;

namespace CoreDocker.Core.Framework.Mappers
{
    public static partial class MapCore
	{
        public static void CreateProjectMap(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Project, ProjectReference>();   
        }

        public static ProjectReference ToReference(this Project project, ProjectReference projectReference = null)
        {
            return Mapper.Map(project, projectReference);
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