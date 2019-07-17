using AutoMapper;
using CoreDocker.Dal.Models.Base;

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

        public static IMappingExpression<T, T2> IgnoreCreateUpdate<T, T2>(this IMappingExpression<T, T2> mappingExpression) where T2 : BaseDalModel
        {
            return mappingExpression
                .ForMember(x => x.CreateDate, opt => opt.Ignore())
                .ForMember(x => x.UpdateDate, opt => opt.Ignore());
        }

        public static void AssertConfigurationIsValid()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}