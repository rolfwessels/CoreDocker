using AutoMapper;
using CoreDocker.Core.Framework.CommandQuery;

namespace CoreDocker.Core.Framework.Mappers
{
    public static partial class MapCore
    {
        static MapCore()
        {
            Mapper.Initialize(cfg =>
            {
                CreateCommandMap(cfg);
                CreateProjectMap(cfg);
                CreateUserMap(cfg);
            });
        }

        public static void AssertConfigurationIsValid()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}