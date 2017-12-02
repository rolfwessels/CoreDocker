using CoreDocker.Sdk.RestApi.Clients;

namespace CoreDocker.Sdk.RestApi
{
    public interface ICoreDockerApi
    {
        ProjectApiClient Projects { get; set; }
        UserApiClient Users { get; set; }
    }
}