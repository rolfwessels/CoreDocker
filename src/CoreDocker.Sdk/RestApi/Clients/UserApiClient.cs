using System.Collections.Generic;
using System.Threading.Tasks;
using MainSolutionTemplate.Sdk.RestApi.Base;
using MainSolutionTemplate.Shared.Interfaces.Shared;
using MainSolutionTemplate.Shared.Models;
using MainSolutionTemplate.Shared.Models.Reference;
using CoreDocker.Shared;
using CoreDocker.Sdk;
using Flurl.Http;

namespace MainSolutionTemplate.Sdk.RestApi
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