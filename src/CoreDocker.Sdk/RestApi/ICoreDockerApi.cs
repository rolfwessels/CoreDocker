using MainSolutionTemplate.Sdk.RestApi;

namespace CoreDocker.Sdk
{
    public interface ICoreDockerApi
    {
        ProjectApiClient Projects { get; set; }
    }
}