using System.Collections.Generic;
using CoreDocker.Dal.Models.Enums;

namespace CoreDocker.Dal.Models
{
	public class Role
	{
		public string Name { get; set; }
		public List<Activity> Activities { get; set; }
	}
}