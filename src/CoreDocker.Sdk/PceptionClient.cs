namespace CoreDocker.Sdk
{
    public class CoreDockerClient : ICoreDockerApi
    {
        private readonly string _urlBase;

        public CoreDockerClient(string urlBase)
        {
            _urlBase = urlBase;
            Configs = new ConfigClient(this);
        }

        public string UrlBase
        {
            get { return _urlBase; }
        }

        #region Implementation of ICoreDockerApi


        public IConfigClient Configs { get; set; }

        #endregion
    }

    
    public interface ICoreDockerApi
    {
        IConfigClient Configs { get; set; }
    }
}