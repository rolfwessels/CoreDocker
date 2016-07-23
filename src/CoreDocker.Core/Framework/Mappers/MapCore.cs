using AutoMapper;

namespace MainSolutionTemplate.Core.Mappers
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