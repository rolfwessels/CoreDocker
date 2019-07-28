namespace CoreDocker.Shared.Models.Shared
{
    public class BaseReferenceModel
    {
        public string Id { get; set; }

        #region Equality members

        protected bool Equals(BaseReferenceModel other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((BaseReferenceModel) obj);
        }

        #endregion
    }
}
