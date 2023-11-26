using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Dal.Persistence;
using CoreDocker.Shared.Models.Projects;
using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Types;

namespace CoreDocker.Api.Components.Projects
{
    public class ProjectsMutation
    {
        private readonly ICommander _commander;
        private readonly IIdGenerator _generator;

        public ProjectsMutation(ICommander commander, IIdGenerator generator)
        {
            _commander = commander;
            _generator = generator;
        }

        public Task<CommandResult> Create(
            [GraphQLNonNullType] [GraphQLType(typeof(NonNullType<ProjectCreateUpdateType>))]
            ProjectCreateUpdateModel project)
        {
            return _commander.Execute(new ProjectCreate.Request(_generator.NewId, project.Name),
                CancellationToken.None);
        }

        [Authorize]
        public Task<CommandResult> Update(
            [GraphQLNonNullType] string id,
            [GraphQLType(typeof(NonNullType<ProjectCreateUpdateType>))]
            ProjectCreateUpdateModel project)
        {
            return _commander.Execute(new ProjectUpdateName.Request(id, project.Name), CancellationToken.None);
        }

        public Task<CommandResult> Remove([GraphQLNonNullType] string id)
        {
            return _commander.Execute(new ProjectRemove.Request(id), CancellationToken.None);
        }
    }
}