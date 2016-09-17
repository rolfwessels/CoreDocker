using AutoMapper;

namespace CoreDocker.Core.Mappers
{
    public static partial class MapCore
    {
        static MapCore()
        {
            Mapper.Initialize(cfg => {
                CreateProjectMap(cfg);
                CreateUserMap(cfg);
            });
        }

        
    }
}