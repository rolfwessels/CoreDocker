using CoreDocker.Sdk.RestApi.Clients;

namespace CoreDocker.Sdk.RestApi
{
    public interface ICoreDockerApi
    {
        ProjectApiClient Projects { get; }
        UserApiClient Users { get; }
        AuthenticateApiClient Authenticate { get; }
        PingApiClient Ping { get; }
    }
}