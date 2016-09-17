using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CoreDocker.Dal.Mongo.Migrations
{
    public class VersionUpdater
    {
        
        private readonly IMigration[] _updates;
        private readonly Mutex _resetEvent = new Mutex(false, @"global/CoreDocker_VersionUpdater");
        private ILogger _log;

        public VersionUpdater(IMigration[] updates, ILogger log)
        {
            _log = log;
            _updates = updates;
        }

        public Task Update(IMongoDatabase db )
        {

            return Task.Run(() =>
            {
                if (_resetEvent.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    
                    var repository = new MongoRepository<DbVersion>(db);
                    List<DbVersion> versions = repository.Find().Result;
                    _log.LogInformation(string.Format("Found {0} database updates in database and {1} in code", versions.Count, _updates.Length));
                    for (int i = 0; i < _updates.Length; i++)
                    {   
                        IMigration migrateInitialize = _updates[i];
                        EnsureThatVersionDoesNotExistThenUpdate(versions, i, migrateInitialize, repository, db).Wait();
                    }
                    _log.LogInformation("Done");
                    
                    _resetEvent.ReleaseMutex();
                }

            });

        }

        

        #region Private Methods

        private async Task EnsureThatVersionDoesNotExistThenUpdate(IEnumerable<DbVersion> versions, int i, IMigration migrateInitialize, MongoRepository<DbVersion> repository, IMongoDatabase db)
        {
            DbVersion version = versions.FirstOrDefault(x => x.Id == i);
            if (version == null)
            {
                _log.LogInformation(string.Format("Running version update {0}", migrateInitialize.GetType().Name));
                await RunTheUpdate(migrateInitialize, db);
                var dbVersion1 = new DbVersion {Id = i, Name = migrateInitialize.GetType().Name};
                await repository.Add(dbVersion1);
            }
        }

        private async Task RunTheUpdate(IMigration migrateInitialize, IMongoDatabase db)
        {
            _log.LogInformation(string.Format("Starting {0} db update", migrateInitialize.GetType().Name));
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await migrateInitialize.Update(db);
            stopwatch.Stop();
            _log.LogInformation(string.Format("Done {0} in {1}ms", migrateInitialize.GetType().Name, stopwatch.ElapsedMilliseconds));
        }

        #endregion
    }
}