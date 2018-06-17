using System.Collections.Generic;
using CoreDocker.Dal.Models.Auth;

namespace CoreDocker.Dal.Models.Users
{
	public class Role
	{
		public string Name { get; set; }
		public List<Activity> Activities { get; set; }
	}
}