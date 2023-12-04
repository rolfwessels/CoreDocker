using System.Reflection;
using CoreDocker.Api.Components;
using CoreDocker.Api.Components.Projects;
using CoreDocker.Api.Components.Users;
using CoreDocker.Api.GraphQl;
using CoreDocker.Api.Security;
using CoreDocker.Core;
using CoreDocker.Dal.MongoDb;
using CoreDocker.Dal.Persistence;


namespace CoreDocker.Api.AppStartup
{ 
    public static class IocApi 
    {

        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);
        
        private static IGeneralUnitOfWorkFactory GetInstanceOfIGeneralUnitOfWorkFactory(IServiceProvider arg)
        {
            _log.Information("Connecting to :{MongoConnection} [{MongoDatabase}]", Settings.Instance.MongoConnection,
                Settings.Instance.MongoDatabase);
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


        public static void AddApiIoc(this IServiceCollection builder)
        {
            builder.AddSingleton(GetInstanceOfIGeneralUnitOfWorkFactory);
            SetupGraphQl(builder);
            SetupTools(builder);
        }

        

        private static void SetupGraphQl(IServiceCollection builder)
        {
            builder.AddSingleton<CommandResultType>();

            builder.AddTransient<IErrorFilter,ErrorFilter>();

            builder.AddTransient<DefaultQuery>();
            builder.AddTransient<DefaultMutation>();
            builder.AddSingleton<DefaultSubscription>();
            builder.AddSingleton<SubscriptionSubscribe>();
            builder.AddTransient<RealTimeNotificationsMessageType>();


            /*user*/
            builder.AddTransient<UserType>();
            builder.AddTransient<UsersQueryType>();
            builder.AddTransient<UserCreateUpdateType>();
            builder.AddTransient<UsersMutationType>();
            builder.AddTransient<RoleType>();
            builder.AddTransient<RegisterType>();

            /*project*/
            builder.AddTransient<ProjectType>();
            builder.AddTransient<OpenIdSettings>();
            builder.AddTransient<ProjectsQueryType>();
            builder.AddTransient<ProjectCreateUpdateType>();
            builder.AddTransient<ProjectsMutation>();
            builder.AddTransient<ProjectsMutationType>();
            builder.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }


        private static void SetupTools(IServiceCollection builder)
        {
            builder.AddSingleton<IIdGenerator, ObjectIdGenerator>();
        }


    

    }
}