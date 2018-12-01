using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using MongoDB.Driver;

namespace CoreDocker.Dal.MongoDb
{
    public class MongoGeneralUnitOfWork : IGeneralUnitOfWork
    {
        public MongoGeneralUnitOfWork(IMongoDatabase database)
        {
            Users = new MongoRepository<User>(database);
            Projects = new MongoRepository<Project>(database);
            UserGrants = new MongoRepository<UserGrant>(database);
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

        #endregion
    }
}