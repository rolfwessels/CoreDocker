﻿using System;
using System.Reflection;
using CoreDocker.Dal.Persistance;
using log4net;
using MongoDB.Driver;

namespace CoreDocker.Dal.MongoDb
{
    public class MongoConnectionFactory : IGeneralUnitOfWorkFactory
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Lazy<IGeneralUnitOfWork> _singleConnection;

        public MongoConnectionFactory(string connectionString, string databaseName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
            _singleConnection = new Lazy<IGeneralUnitOfWork>(GeneralUnitOfWork);
        }

        public string DatabaseName { get; }

        public string ConnectionString { get; }

        #region IGeneralUnitOfWorkFactory Members

        public IGeneralUnitOfWork GetConnection()
        {
            return _singleConnection.Value;
        }

        #endregion

        public IMongoDatabase DatabaseOnly()
        {
            var client = ClientOnly();
            var database = client.GetDatabase(DatabaseName);
            return database;
        }

        #region Private Methods

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

        #endregion
    }
}