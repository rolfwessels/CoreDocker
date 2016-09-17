using CoreDocker.Dal.Models;

namespace CoreDocker.Core.BusinessLogic.Components.Interfaces
{
    public interface IApplicationManager : IBaseManager<Application>
    {
        Application GetApplicationById(string clientId);
    }
}