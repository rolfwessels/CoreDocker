using System;
using MainSolutionTemplate.Dal.Persistance;

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