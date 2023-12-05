namespace CoreDocker.Shared.Models.Shared
{
    public class BaseReferenceModel
    {
        public string Id { get; init; } = null!;

        protected bool Equals(BaseReferenceModel other)
        {
            return Id.Equals(other.Id);
        }
    }
}