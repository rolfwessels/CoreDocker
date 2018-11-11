namespace CoreDocker.Dal.Persistence
{
    public interface IGeneralUnitOfWorkFactory
    {
        IGeneralUnitOfWork GetConnection();
    }
}