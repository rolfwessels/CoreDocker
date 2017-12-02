using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Sdk.Helpers;
using CoreDocker.Sdk.RestApi.Base;
using CoreDocker.Shared;
using CoreDocker.Shared.Interfaces.Shared;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Reference;
using Flurl.Http;
using RestSharp;

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
            var restRequest = new RestRequest(DefaultUrl(RouteHelper.UserControllerRegister),Method.POST);
            var executeAsyncWithLogging = await CoreDockerClient.Client.ExecuteAsyncWithLogging<UserModel>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }

        public async Task<bool> ForgotPassword(string email)
        {
            var restRequest = new RestRequest(DefaultUrl(RouteHelper.UserControllerForgotPassword.SetParam("email", email)));
            var executeAsyncWithLogging = await CoreDockerClient.Client.ExecuteAsyncWithLogging<bool> (restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }

        public async Task<UserModel> WhoAmI()
        {
            var restRequest = new RestRequest(DefaultUrl(RouteHelper.UserControllerWhoAmI));
            var executeAsyncWithLogging = await CoreDockerClient.Client.ExecuteAsyncWithLogging<UserModel>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }

        public async Task<List<RoleModel>> Roles()
        {
            var restRequest = new RestRequest(DefaultUrl(RouteHelper.UserControllerRoles));
            var executeAsyncWithLogging = await CoreDockerClient.Client.ExecuteAsyncWithLogging<List<RoleModel>>(restRequest);
            return ValidateResponse(executeAsyncWithLogging);
        }

        #endregion
    }
}