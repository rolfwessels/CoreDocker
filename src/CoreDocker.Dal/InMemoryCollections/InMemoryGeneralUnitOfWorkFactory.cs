using System;
using CoreDocker.Dal.Persistance;

namespace CoreDocker.Dal.InMemoryCollections
{
    public class InMemoryGeneralUnitOfWorkFactory : IGeneralUnitOfWorkFactory
    {
        private InMemoryGeneralUnitOfWork _inMemoryGeneralUnitOfWork;

        public InMemoryGeneralUnitOfWorkFactory()
        {
            _inMemoryGeneralUnitOfWork = new InMemoryGeneralUnitOfWork();
        }

        public IGeneralUnitOfWork GetConnection()
        {
            return _inMemoryGeneralUnitOfWork;
        }
    }
}