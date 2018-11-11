using AutoMapper;

namespace CoreDocker.Core.Framework.Mappers
{
    public static partial class MapCore
    {
        static MapCore()
        {
            Mapper.Initialize(cfg =>
            {
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