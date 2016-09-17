using CoreDocker.Sdk.RestApi;

namespace CoreDocker.Sdk
{
    public interface ICoreDockerApi
    {
        ProjectApiClient Projects { get; set; }
        UserApiClient Users { get; set; }
    }
}