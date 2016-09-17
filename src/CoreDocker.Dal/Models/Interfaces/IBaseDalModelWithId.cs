using System;

namespace CoreDocker.Dal.Models.Interfaces
{
	public interface IBaseDalModelWithId : IBaseDalModel
	{
        string Id { get; set; }
	}
}