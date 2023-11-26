using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.SystemEvents;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;

namespace CoreDocker.Dal.InMemoryCollections
{
    public class InMemoryGeneralUnitOfWork : IGeneralUnitOfWork
    {
        public InMemoryGeneralUnitOfWork()
        {
            Users = new FakeRepository<User>();
            Projects = new FakeRepository<Project>();
            UserGrants = new FakeRepository<UserGrant>();
            SystemCommands = new FakeRepository<SystemCommand>();
            SystemEvents = new FakeRepository<SystemEvent>();
        }

        #region IGeneralUnitOfWork Members

        #region Implementation of IDisposable

        public void Dispose()
        {
        }

        #endregion

        #endregion


        #region Implementation of IGeneralUnitOfWork

        public IRepository<User> Users { get; }
        public IRepository<Project> Projects { get; }
        public IRepository<UserGrant> UserGrants { get; }
        public IRepository<SystemCommand> SystemCommands { get; set; }
        public IRepository<SystemEvent> SystemEvents { get; set; }

        #endregion
    }
}