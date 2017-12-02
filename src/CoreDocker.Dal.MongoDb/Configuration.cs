using System.Threading.Tasks;
using CoreDocker.Dal.Mongo;
using CoreDocker.Dal.Mongo.Migrations;
using CoreDocker.Dal.Mongo.Migrations.Versions;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CoreDocker.Dal.MongoDb
{
	public class Configuration
	{
	    private static readonly object _locker = new object();
		private static Configuration _instance;
	    private MongoMappers _mongoMappers;
	    private readonly IMigration[] _updates;
	    private Task _update;

	    protected Configuration()
	    {
	        _updates = new IMigration[] {
                new MigrateInitialize()
            };
	    }

	    public Task Update(IMongoDatabase db, ILogger logger)
	    {
	        lock (_instance)
	        {
	            if (_update == null)
	            {
	                _mongoMappers = new MongoMappers();
	                _mongoMappers.InitializeMappers(logger);
	                var versionUpdater = new VersionUpdater(_updates, logger);
	                _update = versionUpdater.Update(db);
	            }
	        }
            return _update;
	    }

	    #region Instance

	    public static Configuration Instance()
	    {
	        if (_instance == null)
	            lock (_locker)
	            {
	                if (_instance == null)
	                {
	                    _instance = new Configuration();
	                }
	            }
	        return _instance;
	    }

	    #endregion

	}
}