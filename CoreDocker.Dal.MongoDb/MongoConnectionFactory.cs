using System;
using System.Linq;
using MainSolutionTemplate.Dal.Persistance;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace MainSolutionTemplate.Dal.Mongo
{
    public class MongoConnectionFactory : IGeneralUnitOfWorkFactory
    {
        
        private readonly string _connectionString;
        private readonly ILogger _logger;
        private readonly string _databaseName;
        private readonly Lazy<IGeneralUnitOfWork> _singleConnection;

        public MongoConnectionFactory(string connectionString , ILogger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
            _databaseName = new Uri(_connectionString).Segments.Skip(1).FirstOrDefault() ?? "MainSolutionTemplate";
            _singleConnection = new Lazy<IGeneralUnitOfWork>(GeneralUnitOfWork);
        }

        public string DatabaseName
        {
            get { return _databaseName; }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
        }

        #region IGeneralUnitOfWorkFactory Members

        public IGeneralUnitOfWork GetConnection()
        {
            return _singleConnection.Value;
        }

        private IGeneralUnitOfWork GeneralUnitOfWork()
        {
            IMongoDatabase database = DatabaseOnly();
            Configuration.Instance().Update(database, _logger).Wait();
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