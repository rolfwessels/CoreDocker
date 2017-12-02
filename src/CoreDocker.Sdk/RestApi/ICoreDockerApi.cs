using CoreDocker.Sdk.RestApi.Clients;
using CoreDocker.Shared.Models;

namespace CoreDocker.Sdk.RestApi
{
    public interface ICoreDockerApi
    {
        ProjectApiClient Projects { get; }
        UserApiClient Users { get; }
        AuthenticateApiClient Authenticate { get; }
        PingApiClient Ping { get; }
        void SetToken(TokenResponseModel data);
    }
}