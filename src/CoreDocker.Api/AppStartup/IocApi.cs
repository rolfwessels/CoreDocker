using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Api.Common;
using CoreDocker.Api.Security;
using CoreDocker.Core.Startup;
using CoreDocker.Dal.MongoDb;
using CoreDocker.Dal.Persistance;
using CoreDocker.Utilities;
using log4net;
using Microsoft.Extensions.DependencyInjection;

namespace CoreDocker.Api.AppStartup
{
    public class IocApi : IocCoreBase
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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
            SecuritySetupClient.Add(builder);
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
            
            _log.Info($"Connecting to :{Settings.Instance.MongoConnection} [{Settings.Instance.MongoDatabase}]");
            try
            {
                return new MongoConnectionFactory(Settings.Instance.MongoConnection, Settings.Instance.MongoDatabase);
            }
            catch (Exception e)
            {
                _log.Error($"Error connecting to the database:{e.Message}", e);
                throw;
            }
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
            builder.RegisterType<UserCommonController>();
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