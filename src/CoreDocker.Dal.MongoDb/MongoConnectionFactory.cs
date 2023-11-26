using System;
using CoreDocker.Dal.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CoreDocker.Dal.MongoDb
{
    public class MongoConnectionFactory : IGeneralUnitOfWorkFactory
    {
        private readonly Lazy<IGeneralUnitOfWork> _singleConnection;

        public MongoConnectionFactory(string connectionString, string databaseName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
            _singleConnection = new Lazy<IGeneralUnitOfWork>(GeneralUnitOfWork);
        }

        public string DatabaseName { get; }

        public string ConnectionString { get; }

        public string NewId => ObjectId.GenerateNewId().ToString();

        public IGeneralUnitOfWork GetConnection()
        {
            return _singleConnection.Value;
        }

        public IMongoDatabase DatabaseOnly()
        {
            var client = ClientOnly();
            var database = client.GetDatabase(DatabaseName);
            return database;
        }

        private IGeneralUnitOfWork GeneralUnitOfWork()
        {
            var database = DatabaseOnly();
            Configuration.Instance().Update(database).Wait();
            return new MongoGeneralUnitOfWork(database);
        }

        private IMongoClient ClientOnly()
        {
            return new MongoClient(ConnectionString);
        }
    }
}