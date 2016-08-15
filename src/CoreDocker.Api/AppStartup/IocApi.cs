using System;
using Autofac;
using MainSolutionTemplate.Api.Common;
using MainSolutionTemplate.Api.WebApi.Controllers;
using MainSolutionTemplate.Core.Startup;
using MainSolutionTemplate.Dal.Persistance;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Dal.InMemoryCollections;

namespace MainSolutionTemplate.Api.AppStartup
{
  public class IocApi : IocCoreBase
  {
    private static bool _isInitialized;
    private static readonly object _locker = new object();
    private static IocApi _instance;
    private static IServiceCollection _services;

    public IocApi()
    {
      var builder = new ContainerBuilder();
      SetupCore(builder);
      SetupCommonControllers(builder);
      SetupTools(builder);
      builder.Populate(_services);
      Container = builder.Build();

    }

    public static void Populate(IServiceCollection services)
    {
      if (_isInitialized) throw new Exception("Need to call Populate before first instance call.");
      _services = services;
    }

    #region Overrides of IocCoreBase

    protected override IGeneralUnitOfWorkFactory GetInstanceOfIGeneralUnitOfWorkFactory(IComponentContext arg)
    {
      return new InMemoryGeneralUnitOfWorkFactory();
    }

    #endregion

    #region Private Methods

    private void SetupCommonControllers(ContainerBuilder builder)
    {
      builder.RegisterType<UserCommonController>();
      builder.RegisterType<ProjectCommonController>();
    }

    private void SetupTools(ContainerBuilder builder)
    {
    }

  

    #endregion

    #region Instance

    public static IocApi Instance
    {
      get
      {
        if (_isInitialized) return _instance;
        lock (_locker)
        {
          if (!_isInitialized)
          {
            _instance = new IocApi();
            _isInitialized = true;
          }
        }
        return _instance;
      }
    }

    public IContainer Container { get; }


    public T Resolve<T>()
    {
      return Container.Resolve<T>();
    }

   

    #endregion
  }
}