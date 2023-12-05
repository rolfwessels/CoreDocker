using System;
using System.Reflection;
using Bumbershoot.Utilities.Serializer;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Event;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Core.Framework.Subscriptions;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.SystemEvents;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using IValidatorFactory = CoreDocker.Dal.Validation.IValidatorFactory;
using ValidatorFactoryBase = CoreDocker.Dal.Validation.ValidatorFactoryBase;

namespace CoreDocker.Core.Startup
{
    public static class IocCore 
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()!.DeclaringType!);

        public static void AddCoreIoc(this IServiceCollection builder)
        { 

            SetupMongoDb(builder);
            SetupManagers(builder);
            SetupTools(builder);
            SetupValidation(builder);
            SetupMediator(builder);
        }

        private static void SetupMediator(IServiceCollection builder)
        {
           
            
        }

        private static void SetupMongoDb(IServiceCollection builder)
        {
            builder.AddTransient(Delegate);
            builder.AddTransient(x => x.GetRequiredService<IGeneralUnitOfWork>().UserGrants);
            builder.AddTransient(x => x.GetRequiredService<IGeneralUnitOfWork>().Projects);
            builder.AddTransient(x => x.GetRequiredService<IGeneralUnitOfWork>().Users);
            builder.AddTransient(x => x.GetRequiredService<IGeneralUnitOfWork>().SystemCommands);
            builder.AddTransient(x => x.GetRequiredService<IGeneralUnitOfWork>().SystemEvents);
        }


        private static IGeneralUnitOfWork Delegate(IServiceProvider provider)
        {
            try
            {
                return provider.GetRequiredService<IGeneralUnitOfWorkFactory>().GetConnection();
            }
            catch (Exception e)
            {
                _log.Error("IocCoreBase:Delegate " + e.Message, e);
                _log.Error(e.Source??"");
                _log.Error(e.StackTrace??"");
                throw;
            }
        }

        private static void SetupManagers(IServiceCollection builder)
        {
            builder.AddTransient<IProjectLookup,ProjectLookup>();
            builder.AddTransient<IRoleManager,RoleManager>();
            builder.AddTransient<IUserLookup,UserLookup>();
            builder.AddTransient<IUserGrantLookup,UserGrantLookup>();
        }

        private static void SetupValidation(IServiceCollection builder)
        {
            builder.AddTransient<IValidatorFactory,ValidatorFactory>();
            builder.AddTransient<IValidator<User>,UserValidator>();
            builder.AddTransient<IValidator<Project>,ProjectValidator>();
            builder.AddTransient<IValidator<UserGrant>,UserGrantValidator>();
            builder.AddTransient<IValidator<User>,UserValidator>();
        }

        private static void SetupTools(IServiceCollection builder)
        {
            builder.AddSingleton<Settings>();
            builder.AddSingleton<IMessenger>(x => new RedisMessenger(x.GetRequiredService<Settings>().RedisHost));
            builder.AddSingleton<MediatorCommander>();
            builder.AddSingleton<SubscriptionNotifications>();
            builder.AddSingleton<IStringify,StringifyJson>();
            builder.AddTransient<ICommander>(x => new CommanderPersist(x.GetRequiredService<MediatorCommander>(), x.GetRequiredService<IRepository<SystemCommand>>(), x.GetRequiredService<IStringify>(), x.GetRequiredService<IEventStoreConnection>()));
            builder.AddTransient<IEventStoreConnection,EventStoreConnection>();
        }

        private class ValidatorFactory : ValidatorFactoryBase
        {
            private readonly IServiceProvider _context;

            public ValidatorFactory(IServiceProvider context)
            {
                _context = context;
            }

            protected override void TryResolve<T>(out IValidator<T> output)
            {
                output = _context.GetService<IValidator<T>>()!;
            }
        }
    }
}