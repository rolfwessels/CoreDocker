using System;
using System.Reflection;
using Autofac;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.MessageUtil;
using CoreDocker.Core.Framework.Subscriptions;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistence;
using FluentValidation;
using log4net;
using MediatR;
using IValidatorFactory = CoreDocker.Dal.Validation.IValidatorFactory;
using ValidatorFactoryBase = CoreDocker.Dal.Validation.ValidatorFactoryBase;

namespace CoreDocker.Core.Startup
{
    public abstract class IocCoreBase
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected void SetupCore(ContainerBuilder builder)
        {
            SetupMongoDb(builder);
            SetupManagers(builder);
            SetupTools(builder);
            SetupValidation(builder);
            SetupMediator(builder);
        }

        private void SetupMediator(ContainerBuilder builder)
        {
            // mediator itself
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            // request & notification handlers
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            builder.RegisterAssemblyTypes(typeof(IocCoreBase).GetTypeInfo().Assembly)
                .Where(t => typeof(INotification).IsAssignableFrom(t) || t.IsClosedTypeOf(typeof(IRequestHandler<,>)) || t.IsClosedTypeOf(typeof(INotificationHandler<>)))
                .AsImplementedInterfaces(); // via assembly scan
        }

        protected virtual void SetupMongoDb(ContainerBuilder builder)
        {
            builder.Register(GetInstanceOfIGeneralUnitOfWorkFactory).SingleInstance();
            builder.Register(Delegate).As<IGeneralUnitOfWork>();
        }

        protected abstract IGeneralUnitOfWorkFactory GetInstanceOfIGeneralUnitOfWorkFactory(IComponentContext arg);

        #region Private Methods

        private IGeneralUnitOfWork Delegate(IComponentContext x)
        {
            try
            {
                return x.Resolve<IGeneralUnitOfWorkFactory>().GetConnection();
            }
            catch (Exception e)
            {
                _log.Error("IocCoreBase:Delegate " + e.Message, e);
                _log.Error(e.Source);
                _log.Error(e.StackTrace);
                throw;
            }
        }

        private static void SetupManagers(ContainerBuilder builder)
        {
            builder.RegisterType<BaseManagerArguments>();
            builder.RegisterType<ProjectLookup>().As<IProjectLookup>();
            builder.RegisterType<RoleManager>().As<IRoleManager>();
            builder.RegisterType<UserLookup>().As<IUserLookup>();
            builder.RegisterType<UserGrantLookup>().As<IUserGrantLookup>();
        }

        private static void SetupValidation(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacValidatorFactory>().As<IValidatorFactory>();
            builder.RegisterType<UserValidator>().As<IValidator<User>>();
            builder.RegisterType<ProjectValidator>().As<IValidator<Project>>();
            builder.RegisterType<UserGrantValidator>().As<IValidator<UserGrant>>();
            builder.RegisterType<UserValidator>().As<IValidator<User>>();
        }

        private void SetupTools(ContainerBuilder builder)
        {
            builder.Register(x => Messenger.Default).As<IMessenger>();
            builder.RegisterType<Commander>().As<ICommander>();
            builder.RegisterType<SubscriptionNotifications>().SingleInstance();
        }

        #endregion

        #region Nested type: AutofacValidatorFactory

        private class AutofacValidatorFactory : ValidatorFactoryBase
        {
            private readonly Func<IComponentContext> _context;

            public AutofacValidatorFactory(Func<IComponentContext> context)
            {
                _context = context;
            }

            protected override void TryResolve<T>(out IValidator<T> output)
            {
                _context().TryResolve(out output);
            }
        }

        #endregion
    }
}