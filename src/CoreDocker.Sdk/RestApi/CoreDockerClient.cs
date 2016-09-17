using CoreDocker.Sdk.RestApi;

namespace CoreDocker.Sdk
{
    public class CoreDockerClient : ICoreDockerApi
    {
        private readonly string _urlBase;

        public CoreDockerClient(string urlBase)
        {
            _urlBase = urlBase;
            Projects = new ProjectApiClient(this);
            Users = new UserApiClient(this);
        }

        public string UrlBase
        {
            get { return _urlBase; }
        }

        #region Implementation of ICoreDockerApi


        public ProjectApiClient Projects { get; set; }
        public UserApiClient Users { get; set; }

        #endregion
    }
}