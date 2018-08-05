using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Persistance;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Core.Components.Applications
{
    public class ApplicationManager : BaseManager<Application>, IApplicationManager
    {
        public ApplicationManager(BaseManagerArguments baseManagerArguments , ILogger<ApplicationManager> log ) : base(baseManagerArguments , log)
        {
        }

        protected override IRepository<Application> Repository
        {
            get { return _generalUnitOfWork.Applications; }
        }

        #region IApplicationManager Members

        public Application GetApplicationById(string clientId)
        {
            return _generalUnitOfWork.Applications.FindOne(x => x.ClientId == clientId).Result;
        }

        #endregion
    }
}