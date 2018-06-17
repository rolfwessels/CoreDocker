using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistance;
using MongoDB.Driver;

namespace CoreDocker.Dal.MongoDb
{
	public class MongoGeneralUnitOfWork : IGeneralUnitOfWork
	{
	    public MongoGeneralUnitOfWork(IMongoDatabase database)
	    {
            Users = new MongoRepository<User>(database);
            Applications = new MongoRepository<Application>(database);
            Projects = new MongoRepository<Project>(database);
            UserGrants = new MongoRepository<UserGrant>(database);
	    }

		#region Implementation of IDisposable

		public void Dispose()
		{
			
		}

		#endregion

		#region Implementation of IGeneralUnitOfWork

		public IRepository<User> Users { get; private set; }
		public IRepository<Application> Applications { get; private set; }
	    public IRepository<Project> Projects { get; private set; }
	    public IRepository<UserGrant> UserGrants { get; private set; }
	   
	    #endregion

	}

}
