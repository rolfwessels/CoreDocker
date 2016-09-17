namespace CoreDocker.Dal.Persistance
{
    public interface IGeneralUnitOfWorkFactory
    {
        IGeneralUnitOfWork GetConnection();
    }
}