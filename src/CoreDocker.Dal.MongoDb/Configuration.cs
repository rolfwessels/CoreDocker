using System.Threading.Tasks;
using CoreDocker.Dal.MongoDb.Migrations;
using CoreDocker.Dal.MongoDb.Migrations.Versions;
using MongoDB.Driver;
using SharpCompress;

namespace CoreDocker.Dal.MongoDb
{
    public class Configuration
    {
        private static readonly object _locker = new();
        private static Lazy<Configuration> _instance = new(() => new Configuration());
        private readonly IMigration[] _updates;
        private MongoMappers? _mongoMappers;
        private Task? _update;

        protected Configuration()
        {
            _updates = new IMigration[]
            {
                new MigrateInitialize()
            };
        }

        public Task Update(IMongoDatabase db)
        {
            lock (_locker)
            {
                if (_update == null)
                {
                    _mongoMappers = new MongoMappers();
                    _mongoMappers.InitializeMappers();
                    var versionUpdater = new VersionUpdater(_updates);
                    _update = versionUpdater.Update(db);
                }
            }

            return _update;
        }

        #region Instance

        public static Configuration Instance()
        {
            return _instance.Value;
        }

        #endregion
    }
}