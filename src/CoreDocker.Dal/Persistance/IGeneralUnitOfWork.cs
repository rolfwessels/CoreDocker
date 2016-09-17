using System;
using CoreDocker.Dal.Models;

namespace CoreDocker.Dal.Persistance
{
	public interface IGeneralUnitOfWork : IDisposable
	{
		IRepository<User> Users { get;  }
		IRepository<Application> Applications { get; }
		IRepository<Project> Projects { get; }
	}
}