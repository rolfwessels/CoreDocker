using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Shared.Models.Projects;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsMutation
    {
        private readonly ICommander _commander;

        public ProjectsMutation(ICommander commander)
        {
            _commander = commander;
        }

        public Task<CommandResult> Create([GraphQLNonNullType]
            [GraphQLType(typeof(NonNullType<ProjectCreateUpdateType>))]
            ProjectCreateUpdateModel project)
        {
            return _commander.Execute(ProjectCreate.Request.From(_commander.NewId, project.Name));
        }

        [Authorize]
        public Task<CommandResult> Update(
            [GraphQLNonNullType] string id,
            [GraphQLType(typeof(NonNullType<ProjectCreateUpdateType>))]
            ProjectCreateUpdateModel project)
        {
            return _commander.Execute(ProjectUpdate.Request.From(id, project.Name));
        }

        public Task<CommandResult> Remove([GraphQLNonNullType] string id)
        {
            return _commander.Execute(ProjectRemove.Request.From(id));
        }
    }
}