using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Auth;

namespace CoreDocker.Sdk.RestApi
{
    public interface ICoreDockerClient
    {
        ProjectApiClient Projects { get; }
        UserApiClient Users { get; }
        AuthenticateApiClient Authenticate { get; }
        PingApiClient Ping { get; }
        void SetToken(TokenResponseModel data);
        
    }
}