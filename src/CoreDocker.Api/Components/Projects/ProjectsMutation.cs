using System.Threading.Tasks;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Shared.Models.Projects;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsMutation
    {
        private readonly ICommander _commander;

        public ProjectsMutation(ICommander commander)
        {
            _commander = commander;
        }

        public Task<CommandResult> Create(ProjectCreateUpdateModel project)
        {
            return _commander.Execute(ProjectCreate.Request.From(_commander.NewId, project.Name));
        }

        public Task<CommandResult> Update(string id, ProjectCreateUpdateModel project)
        {
            return _commander.Execute(ProjectUpdate.Request.From(id, project.Name));
        }

        public Task<CommandResult> Remove(string id)
        {
            return _commander.Execute(ProjectRemove.Request.From(id));
        }
    }
}