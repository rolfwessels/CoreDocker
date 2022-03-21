using System;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreDocker.Api.Components;
using CoreDocker.Api.Components.Projects;
using CoreDocker.Api.Components.Users;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.Security;
using CoreDocker.Core;
using CoreDocker.Core.Startup;
using CoreDocker.Dal.MongoDb;
using CoreDocker.Dal.Persistence;
using HotChocolate;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CoreDocker.Api.AppStartup
{
    public class IocApi : IocCoreBase
    {

        private static readonly object _locker = new();
        private static IocApi? _instance;
        private static IServiceCollection? _services;
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        public IocApi()
        {
            var builder = new ContainerBuilder();
            SetupCore(builder);
            SetupGraphQl(builder);
            SetupTools(builder);
            builder.Populate(_services);
            Container = builder.Build();
        }

        public static void Populate(IServiceCollection services)
        {
            if (_instance != null)
            {
                throw new Exception("Need to call Populate before first instance call.");
            }

            _services = services;
        }

        #region Overrides of IocCoreBase

        protected override IGeneralUnitOfWorkFactory GetInstanceOfIGeneralUnitOfWorkFactory(IComponentContext arg)
        {
            _log.Information($"Connecting to :{Settings.Instance.MongoConnection} [{Settings.Instance.MongoDatabase}]");
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

        private static void SetupGraphQl(ContainerBuilder builder)
        {
            builder.RegisterType<CommandResultType>().SingleInstance();

            builder.RegisterType<ErrorFilter>().As<IErrorFilter>();

            builder.RegisterType<DefaultQuery>();
            builder.RegisterType<DefaultMutation>();
            builder.RegisterType<DefaultSubscription>().SingleInstance();
            builder.RegisterType<SubscriptionSubscribe>().SingleInstance();
            builder.RegisterType<RealTimeNotificationsMessageType>();


            /*user*/
            builder.RegisterType<UserType>();
            builder.RegisterType<UsersQueryType>();
            builder.RegisterType<UserCreateUpdateType>();
            builder.RegisterType<UsersMutationType>();
            builder.RegisterType<RoleType>();
            builder.RegisterType<RegisterType>();

            /*project*/
            builder.RegisterType<ProjectType>();
            builder.RegisterType<OpenIdSettings>();
            builder.RegisterType<ProjectsQueryType>();
            builder.RegisterType<ProjectCreateUpdateType>();
            builder.RegisterType<ProjectsMutation>();
            builder.RegisterType<ProjectsMutationType>();


            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
        }

        

        private void SetupTools(ContainerBuilder builder)
        {
            builder.RegisterType<ObjectIdGenerator>().As<IIdGenerator>().SingleInstance();
        }

        #endregion

        #region Instance

        public static IocApi Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                lock (_locker)
                {
                    _instance ??= new IocApi();
                }
                return _instance;
            }
        }

        public IContainer Container { get; }


        public T Resolve<T>() where T : notnull
        {
            return Container.Resolve<T>();
        }

        #endregion
    }
}