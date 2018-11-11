using CoreDocker.Dal.Persistance;

namespace CoreDocker.Dal.InMemoryCollections
{
    public class InMemoryGeneralUnitOfWorkFactory : IGeneralUnitOfWorkFactory
    {
        private readonly InMemoryGeneralUnitOfWork _inMemoryGeneralUnitOfWork;

        public InMemoryGeneralUnitOfWorkFactory()
        {
            _inMemoryGeneralUnitOfWork = new InMemoryGeneralUnitOfWork();
        }

        #region IGeneralUnitOfWorkFactory Members

        public IGeneralUnitOfWork GetConnection()
        {
            return _inMemoryGeneralUnitOfWork;
        }

        #endregion
    }
}