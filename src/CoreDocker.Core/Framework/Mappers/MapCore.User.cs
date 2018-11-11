using System;
using System.Globalization;
using AutoMapper;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Vendor;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Core.Framework.Mappers
{
    public static partial class MapCore
    {
        public static void CreateUserMap(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<User, UserReference>();
            cfg.CreateMap<UserCreate.Request, User>()
                .ForMember(x => x.HashedPassword, opt => opt.MapFrom(x => PasswordHash.CreateHash(x.Password ??  Guid.NewGuid().ToString())))
                .ForMember(x => x.LastLoginDate, opt => opt.Ignore())
                .ForMember(x => x.DefaultProject, opt => opt.Ignore())
                .IgnoreCreateUpdate();
            cfg.CreateMap<UserCreate.Request, UserCreate.Notification>();
        }

        public static IMappingExpression<T, T2> IgnoreCreateUpdate<T,T2>(this IMappingExpression<T, T2> mappingExpression) where T2 :BaseDalModel
        {
            return mappingExpression
                .ForMember(x => x.CreateDate, opt => opt.Ignore())
                .ForMember(x => x.UpdateDate, opt => opt.Ignore());
        }

        public static UserReference ToReference(this User user, UserReference userReference = null)
        {
            return Mapper.Map(user, userReference);
        }

        public static User ToDao(this UserCreate.Request user, User userReference = null)
        {
            return Mapper.Map(user, userReference);
        }
        public static UserCreate.Notification ToEvent(this UserCreate.Request user, UserCreate.Notification userReference = null)
        {
            return Mapper.Map(user, userReference);
        }
    }
}