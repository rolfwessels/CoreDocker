using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Dal.Models.Auth;

namespace CoreDocker.Core.Components.Applications
{
    public interface IApplicationManager : IBaseManager<Application>
    {
        Application GetApplicationById(string clientId);
    }
}