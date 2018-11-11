using System;
using System.Globalization;
using AutoMapper;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Vendor;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Utilities.Helpers;

namespace CoreDocker.Core.Framework.Mappers
{
    public static partial class MapCore
    {
        public static void CreateUserMap(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<User, UserReference>();
            cfg.CreateMap<UserCreate.Request, User>()
                .ForMember(x => x.HashedPassword, opt => opt.MapFrom(x => UserDalHelper.SetPassword(x.Password)))
                .ForMember(x => x.LastLoginDate, opt => opt.Ignore())
                .ForMember(x => x.DefaultProject, opt => opt.Ignore())
                .IgnoreCreateUpdate();
            cfg.CreateMap<UserCreate.Request, UserCreate.Notification>();

            cfg.CreateMap<UserUpdate.Request, User>()
                .ForMember(x => x.HashedPassword, opt => opt.Ignore())
                .ForMember(x => x.LastLoginDate, opt => opt.Ignore())
                .ForMember(x => x.DefaultProject, opt => opt.Ignore())
                .AfterMap((request, user) =>
                {
                    if (request.Password != null) user.SetPassword(request.Password);
                })
                .IgnoreCreateUpdate();

            cfg.CreateMap<UserUpdate.Request, UserUpdate.Notification>()
                .ForMember(x => x.PasswordChanged, opt => opt.MapFrom(x => x.Password != null));

            cfg.CreateMap<UserRemove.Request, UserRemove.Notification>()
                .ForMember(x => x.WasRemoved, opt => opt.Ignore());
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

        public static User ToDao(this UserUpdate.Request user, User userReference = null)
        {
            return Mapper.Map(user, userReference);
        }

        public static UserUpdate.Notification ToEvent(this UserUpdate.Request user, UserUpdate.Notification userReference = null)
        {
            return Mapper.Map(user, userReference);
        }


        public static UserRemove.Notification ToEvent(this UserRemove.Request user, bool wasRemoved,
            UserRemove.Notification userReference = null)
        {
            var notification = Mapper.Map(user, userReference);
            notification.WasRemoved = wasRemoved;
            return notification;
        }
    }
}