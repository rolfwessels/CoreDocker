using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared;
using CoreDocker.Shared.Interfaces.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Reference;
using Flurl.Http;

namespace CoreDocker.Sdk.RestApi.Clients
{
    public class UserApiClient : BaseCrudApiClient<UserModel, UserCreateUpdateModel, UserReferenceModel>,
                                 IUserControllerActions
    {
        public UserApiClient(CoreDockerClient dockerClient)
            : base(dockerClient, RouteHelper.UserController)
        {
        }

        #region Implementation of IUserControllerActions

        public async Task<UserModel> Register(RegisterModel user)
        {
            return await DefaultUrl(RouteHelper.UserControllerRegister)
              .PostJsonAsyncAndLog(user)
              .ReceiveJson<UserModel>();
        }

        public async Task<bool> ForgotPassword(string email)
        {
            return await DefaultUrl(RouteHelper.UserControllerForgotPassword.SetParam("email", email))
              .GetAsyncAndLog()
              .ReceiveJson<bool>();
        }

        public async Task<UserModel> WhoAmI()
        {
            return await DefaultUrl(RouteHelper.UserControllerWhoAmI)
             .GetAsyncAndLog()
             .ReceiveJson<UserModel>();
        }

        public Task<List<RoleModel>> Roles()
        {
            return DefaultUrl(RouteHelper.UserControllerRoles)
            .GetAsyncAndLog()
            .ReceiveJson<List<RoleModel>>();
        }

        #endregion
    }
}