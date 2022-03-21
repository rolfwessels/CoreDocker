namespace CoreDocker.Dal.Models.Base
{
    public abstract class BaseDalModelWithId : BaseDalModel, IBaseDalModelWithId
    {
        public string Id { get; set; } = null!;
    }
}