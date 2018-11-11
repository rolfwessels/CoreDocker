using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Api.Mappers;
using CoreDocker.Api.WebApi.Controllers;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Shared.Interfaces.Shared;
using CoreDocker.Shared.Models.Users;
using log4net;
using Microsoft.AspNetCore.Http;

namespace CoreDocker.Api.Components.Users
{
    public class UserCommonController :
        BaseCommonController<User, UserModel, UserReferenceModel, UserCreateUpdateModel>,
        IUserControllerActions
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICommander _commander;
        private readonly IRoleManager _roleManager;
        private readonly IUserManager _userManager;

        public UserCommonController(IUserManager userManager, IRoleManager roleManager,
            IHttpContextAccessor httpContextAccessor, ICommander commander) : base(userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            _commander = commander;
        }

        #region IUserControllerActions Members

        #region Implementation of ICrudController<UserModel,in UserCreateUpdateModel>

        public override async Task<UserModel> Insert(UserCreateUpdateModel model)
        {
            var commandResult = await _commander.Execute(UserCreate.Request.From(_commander.NewId, model.Name, model.Email, model.Password,model.Roles));
            var user = await _userManager.GetById(commandResult.Id);
            return user.ToModel();
        }

        #endregion

        public async Task<UserModel> Register(RegisterModel model)
        {
            var user = model.ToDal();
            user.Roles.Add(RoleManager.Guest.Name);
            var savedUser = await _userManager.Save(user, model.Password);
            return savedUser.ToModel();
        }

        public Task<bool> ForgotPassword(string email)
        {
            return Task.Run(() =>
            {
                _log.Warn($"User has called forgot password. We should send him and email to [{email}].");
                return true;
            });
        }

        public async Task<List<RoleModel>> Roles()
        {
            var roles = await _roleManager.Get();
            return roles.ToModels().ToList();
        }

        public async Task<UserModel> WhoAmI()
        {
            var email = _httpContextAccessor.GetName();
            if (string.IsNullOrEmpty(email)) return null;
            var whoAmI = await _userManager.GetUserByEmail(email);
            return whoAmI.ToModel();
        }

        #endregion

        #region Overrides of BaseCommonController<User,UserModel,UserReferenceModel,UserCreateUpdateModel>

        protected override async Task<User> AddAdditionalMappings(UserCreateUpdateModel model, User dal)
        {
            var addAdditionalMappings = await base.AddAdditionalMappings(model, dal);

            if (model.Roles != null && model.Roles.Any())
            {
                var roles = await _roleManager.Get();
                var roleLookup = roles.ToDictionary(x => x.Name.ToLower());
                addAdditionalMappings.Roles.Clear();
                addAdditionalMappings.Roles.AddRange(model.Roles
                    .Where(x => roleLookup.ContainsKey(x.ToLower()))
                    .Select(x => roleLookup[x.ToLower()])
                    .Select(x => x.Name)
                );
            }

            if (!addAdditionalMappings.Roles.Any()) addAdditionalMappings.Roles.Add(RoleManager.Guest.Name);

            return addAdditionalMappings;
        }

        #endregion
    }
}