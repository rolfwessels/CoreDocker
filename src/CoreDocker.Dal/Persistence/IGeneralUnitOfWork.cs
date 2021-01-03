using System;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.SystemEvents;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Dal.Persistence
{
    public interface IGeneralUnitOfWork : IDisposable
    {
        IRepository<Project> Projects { get; }
        IRepository<User> Users { get; }
        IRepository<UserGrant> UserGrants { get; }
        IRepository<SystemCommand> SystemCommands { get; set; }
        IRepository<SystemEvent> SystemEvents { get; set; }
    }
}