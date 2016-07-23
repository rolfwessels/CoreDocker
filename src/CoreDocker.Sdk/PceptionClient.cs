namespace CoreDocker.Sdk
{
    public class CoreDockerClient : ICoreDockerApi
    {
        private readonly string _urlBase;

        public CoreDockerClient(string urlBase)
        {
            _urlBase = urlBase;
            Projects = new ProjectClient(this);
        }

        public string UrlBase
        {
            get { return _urlBase; }
        }

        #region Implementation of ICoreDockerApi


        public ProjectClient Projects { get; set; }

        #endregion
    }

    
    public interface ICoreDockerApi
    {
        ProjectClient Projects { get; set; }
    }
}