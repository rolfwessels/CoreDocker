using CoreDocker.Dal.Models.Base;

namespace CoreDocker.Dal.Models.Users
{
	public class UserGrant : BaseDalModelWithId
	{
		public string Name { get; set; }

	    public override string ToString()
	    {
	        return $"UserGrant: {Name}";
	    }
	}
}