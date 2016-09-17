using CoreDocker.Dal.Models;
using CoreDocker.Dal.Persistance;
using MongoDB.Driver;

namespace CoreDocker.Dal.Mongo
{
	public class MongoGeneralUnitOfWork : IGeneralUnitOfWork
	{
	    public MongoGeneralUnitOfWork(IMongoDatabase database)
	    {
            Users = new MongoRepository<User>(database);
            Applications = new MongoRepository<Application>(database);
            Projects = new MongoRepository<Project>(database);
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
	   
	    #endregion

	}

}
