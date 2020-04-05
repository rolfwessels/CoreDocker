using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Models.Projects;

namespace CoreDocker.Core.Components.Projects
{
    public interface IProjectLookup : IBaseLookup<Project>
    {
        Task<PagedList<Project>> GetPaged(ProjectPagedLookupOptions options);
    }
}