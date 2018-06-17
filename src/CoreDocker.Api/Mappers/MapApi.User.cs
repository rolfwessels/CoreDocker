using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Users;

namespace CoreDocker.Api.Models.Mappers
{
    public static partial class MapApi
    {
        public static User ToDal(this UserCreateUpdateModel model, User user = null)
        {
            return GetInstance().Map(model, user);
        }

        public static User ToDal(this RegisterModel model, User user = null)
        {
            return GetInstance().Map(model, user);
        }

        public static UserModel ToModel(this User user, UserModel model = null)
        {
            return GetInstance().Map(user, model);
        }

        public static IEnumerable<UserReferenceModel> ToReferenceModelList(IEnumerable<User> users)
        {
            return GetInstance().Map<IEnumerable<User>, IEnumerable<UserReferenceModel>>(users);
        }

        public static IEnumerable<UserModel> ToModelList(IEnumerable<User> users)
        {
            return GetInstance().Map<IEnumerable<User>, IEnumerable<UserModel>>(users);
        }

        public static IEnumerable<RoleModel> ToModels(this IEnumerable<Role> roles)
        {
            return GetInstance().Map<IEnumerable<Role>, IEnumerable<RoleModel>>(roles);
        }

        #region Private Methods

        private static void MapUserModel(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<User, UserModel>();
            configuration.CreateMap<Role, RoleModel>();
            configuration.CreateMap<User, UserReferenceModel>();
            configuration.CreateMap<UserReference, UserReferenceModel>();

            configuration.CreateMap<UserCreateUpdateModel, User>()
                .ForMember(x => x.Email, opt => opt.MapFrom(x => x.Email.ToLower()))
                .ForMember(x => x.LastLoginDate, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Roles, opt => opt.Ignore())
                .ForMember(x => x.HashedPassword, opt => opt.Ignore())
                .ForMember(x => x.DefaultProject, opt => opt.Ignore())
                .ForMember(x => x.CreateDate, opt => opt.Ignore())
                .ForMember(x => x.UpdateDate, opt => opt.Ignore());

            configuration.CreateMap<RegisterModel, User>()
                .ForMember(x => x.Email, opt => opt.MapFrom(x => x.Email.ToLower()))
                .ForMember(x => x.LastLoginDate, opt => opt.Ignore())
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.Roles, opt => opt.Ignore())
                .ForMember(x => x.LastLoginDate, opt => opt.Ignore())
                .ForMember(x => x.HashedPassword, opt => opt.Ignore())
                .ForMember(x => x.DefaultProject, opt => opt.Ignore())
                .ForMember(x => x.CreateDate, opt => opt.Ignore())
                .ForMember(x => x.UpdateDate, opt => opt.Ignore());
        }

        #endregion
    }
}