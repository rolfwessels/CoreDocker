using MainSolutionTemplate.Dal.Persistance;
using System;

namespace MainSolutionTemplate.Api.AppStartup
{
    internal class MongoConnectionFactory : IGeneralUnitOfWorkFactory
    {
        private object connection;

        public MongoConnectionFactory(object connection)
        {
            this.connection = connection;
        }

        public IGeneralUnitOfWork GetConnection()
        {
            throw new NotImplementedException();
        }
    }
}