using AutoMapper;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Reference;

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

        public static void AssertConfigurationIsValid()
        {
            Mapper.AssertConfigurationIsValid();
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