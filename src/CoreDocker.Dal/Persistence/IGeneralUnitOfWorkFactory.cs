namespace CoreDocker.Dal.Persistence
{
    public interface IGeneralUnitOfWorkFactory
    {
        IGeneralUnitOfWork GetConnection();
        string NewId { get; }
    }
}