using System;
using System.Reflection;
using CoreDocker.Dal.Persistance;
using log4net;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace CoreDocker.Dal.MongoDb
{
    public class MongoConnectionFactory : IGeneralUnitOfWorkFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _connectionString;
        
        private readonly string _databaseName;
        private readonly Lazy<IGeneralUnitOfWork> _singleConnection;

        public MongoConnectionFactory(string connectionString, string databaseName)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _singleConnection = new Lazy<IGeneralUnitOfWork>(GeneralUnitOfWork);
        }

        public string DatabaseName => _databaseName;

        public string ConnectionString => _connectionString;

        #region IGeneralUnitOfWorkFactory Members

        public IGeneralUnitOfWork GetConnection()
        {
            return _singleConnection.Value;
        }

        private IGeneralUnitOfWork GeneralUnitOfWork()
        {
            IMongoDatabase database = DatabaseOnly();
            Configuration.Instance().Update(database).Wait();
            return new MongoGeneralUnitOfWork(database);
        }

        #endregion

        public IMongoDatabase DatabaseOnly()
        {
            IMongoClient client = ClientOnly();
            IMongoDatabase database = client.GetDatabase(_databaseName);
            return database;
        }

        #region Private Methods

        private IMongoClient ClientOnly()
        {
            return new MongoClient(_connectionString);
        }

        #endregion
    }
}