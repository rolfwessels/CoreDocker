using System;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;

namespace CoreDocker.Dal.Persistance
{
    public interface IGeneralUnitOfWork : IDisposable
    {
        IRepository<Application> Applications { get; }
        IRepository<Project> Projects { get; }
        IRepository<User> Users { get; }
        IRepository<UserGrant> UserGrants { get; }
    }
}