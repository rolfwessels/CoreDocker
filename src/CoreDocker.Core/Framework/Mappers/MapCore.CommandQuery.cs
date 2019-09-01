using AutoMapper;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Projects;

namespace CoreDocker.Core.Framework.Mappers
{
    public static partial class MapCore
    {
        private static void CreateCommandMap(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<CommandRequestBase, CommandResult>();
        }

        public static CommandResult ToCommandResult(this CommandRequestBase project,
            CommandResult projectReference = null)
        {
            return GetInstance().Map(project, projectReference);
        }
    }
}
