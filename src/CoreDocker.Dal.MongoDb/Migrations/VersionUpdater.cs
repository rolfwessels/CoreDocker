using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CoreDocker.Dal.MongoDb.Migrations
{
    public class VersionUpdater
    {
        private readonly IMigration[] _updates;
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly object _locker = new object();

        public VersionUpdater(IMigration[] updates)
        {
            _updates = updates;
        }

        public Task Update(IMongoDatabase db )
        {

            return Task.Run(() =>
            {
                lock (_locker)
                {

                    var repository = new MongoRepository<DbVersion>(db);
                    List<DbVersion> versions = repository.Find().Result;
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    for (int i = 0; i < _updates.Length; i++)
                    {   
                        IMigration migrateInitialize = _updates[i];
                        EnsureThatVersionDoesNotExistThenUpdate(versions, i, migrateInitialize, repository, db).Wait();
                    }
                    stopwatch.Stop();
                    _log.Info($"Found {versions.Count} database updates in database and {_updates.Length} in code. Update took [{stopwatch.ElapsedMilliseconds}]");
                }
            });

        }


        #region Private Methods

        private async Task EnsureThatVersionDoesNotExistThenUpdate(IEnumerable<DbVersion> versions, int i, IMigration migrateInitialize, MongoRepository<DbVersion> repository, IMongoDatabase db)
        {
            DbVersion version = versions.FirstOrDefault(x => x.Id == i);
            if (version == null)
            {
                _log.Info($"Running version update {migrateInitialize.GetType().Name}");
                await RunTheUpdate(migrateInitialize, db);
                var dbVersion1 = new DbVersion {Id = i, Name = migrateInitialize.GetType().Name};
                await repository.Add(dbVersion1);
            }
        }

        private async Task RunTheUpdate(IMigration migrateInitialize, IMongoDatabase db)
        {
            _log.Info($"Starting {migrateInitialize.GetType().Name} db update");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await migrateInitialize.Update(db);
            stopwatch.Stop();
            _log.Info($"Done {migrateInitialize.GetType().Name} in {stopwatch.ElapsedMilliseconds}ms");
        }

        #endregion
    }
}