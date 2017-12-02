using CoreDocker.Dal.Models.Interfaces;

namespace CoreDocker.Dal.Models.Base
{
	public abstract class BaseDalModelWithId : BaseDalModel, IBaseDalModelWithId
	{
		public BaseDalModelWithId()
		{
		}

		public string Id { get; set; }
	}
}