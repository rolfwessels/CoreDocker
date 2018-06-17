using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.Mappers;
using CoreDocker.Api.WebApi.Controllers;
using CoreDocker.Core.Components.Users;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Shared.Interfaces.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Users;
using log4net;
using Microsoft.AspNetCore.Http;

namespace CoreDocker.Api.Components.Users
{
    public class UserCommonController : BaseCommonController<User, UserModel, UserReferenceModel, UserCreateUpdateModel>,
                                        IUserControllerActions
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUserManager _userManager;
        private readonly IRoleManager _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserCommonController(IUserManager userManager, IRoleManager roleManager, IHttpContextAccessor httpContextAccessor) : base(userManager)
        {
            
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        #region IUserControllerActions Members

        public async Task<UserModel> Register(RegisterModel model)
        {
            User user = model.ToDal();
            user.Roles.Add(RoleManager.Guest.Name);
            User savedUser = await _userManager.Save(user, model.Password);
            return savedUser.ToModel();
        }

        public Task<bool> ForgotPassword(string email)
        {
            return Task.Run(() =>
                {

                    _log.Warn(string.Format("User has called forgot password. We should send him and email to [{0}].",email));
                    return true;
                    
                });
        }

        #endregion

        #region Overrides of BaseCommonController<User,UserModel,UserReferenceModel,UserCreateUpdateModel>

        protected override async Task<User> AddAdditionalMappings(UserCreateUpdateModel model, User dal)
        {
            var addAdditionalMappings = await base.AddAdditionalMappings(model, dal);
            if (!addAdditionalMappings.Roles.Any()) addAdditionalMappings.Roles.Add(RoleManager.Guest.Name);
            return addAdditionalMappings;
        }

        #endregion

        public async Task<List<RoleModel>> Roles()
        {
            var roles = await _roleManager.Get();
            return roles.ToModels().ToList();
        }

        public async Task<UserModel> WhoAmI()
        {
            var whoAmI = await _userManager.GetUserByEmail(_httpContextAccessor.GetName());
            return whoAmI.ToModel();
            
        }
    }
}