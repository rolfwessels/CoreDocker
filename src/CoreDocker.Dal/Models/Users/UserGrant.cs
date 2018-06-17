using System;
using CoreDocker.Dal.Models.Base;

namespace CoreDocker.Dal.Models.Users
{
	public class UserGrant : BaseDalModelWithId
	{
	    public string Key { get; set; }
	    public string Type { get; set; }
	    public UserReference User { get; set; }
	    public string ClientId { get; set; }
	    public DateTime? Expiration { get; set; }
	    public string Data { get; set; }

        public override string ToString()
	    {
	        return $"UserGrant: {Key.Substring(0,5)}";
	    }
	}
}