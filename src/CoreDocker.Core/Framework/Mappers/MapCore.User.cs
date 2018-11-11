using AutoMapper;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Core.Framework.Mappers
{
    public static partial class MapCore
    {
        public static void CreateUserMap(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<User, UserReference>();
        }

        public static UserReference ToReference(this User user, UserReference userReference = null)
        {
            return Mapper.Map(user, userReference);
        }
    }
}