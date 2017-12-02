using CoreDocker.Sdk.RestApi;
using CoreDocker.Sdk.RestApi.Clients;

namespace CoreDocker.Sdk
{
    public interface ICoreDockerApi
    {
        ProjectApiClient Projects { get; set; }
        UserApiClient Users { get; set; }
    }
}