namespace CoreDocker.Dal.Persistence
{
    public interface IIdGenerator
    {
        string NewId { get; }
    }
}