using System;
using Autofac;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Framework.MessageUtil;
using FluentValidation;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Persistance;
using CoreDocker.Dal.Validation;
using CoreDocker.Dal.Validation.Validators;
using CoreDocker.Utilities.FakeLogging;
using ValidatorFactoryBase = CoreDocker.Dal.Validation.ValidatorFactoryBase;

namespace CoreDocker.Core.Startup
{
	public abstract class IocCoreBase
	{
		protected void SetupCore(ContainerBuilder builder)
		{
            SetupMongoDb(builder);
		    SetupManagers(builder);
			SetupTools(builder);
            SetupValidation(builder);
		}

	    protected virtual void SetupMongoDb(ContainerBuilder builder)
	    {
	        builder.Register(GetInstanceOfIGeneralUnitOfWorkFactory).SingleInstance();
	        builder.Register(Delegate).As<IGeneralUnitOfWork>();
	    }

	    private IGeneralUnitOfWork Delegate(IComponentContext x)
	    {
	        try
	        {
	            return x.Resolve<IGeneralUnitOfWorkFactory>().GetConnection();
	        }
	        catch (Exception e)
	        {
	            var logger = LogManager.GetLogger<IocCoreBase>();
	            logger.Error("IocCoreBase:Delegate " + e.Message, e);
	            logger.Error(e.Source);
                logger.Error(e.StackTrace);
	            throw;
	        }
	    }

	    private static void SetupManagers(ContainerBuilder builder)
		{
            builder.RegisterType<BaseManagerArguments>();
            builder.RegisterType<ProjectManager>().As<IProjectManager>();
            builder.RegisterType<RoleManager>().As<IRoleManager>();
            builder.RegisterType<UserManager>().As<IUserManager>();
		}

	    private static void SetupValidation(ContainerBuilder builder)
	    {
            builder.RegisterType<AutofacValidatorFactory>().As<Dal.Validation.IValidatorFactory>();
	        builder.RegisterType<UserValidator>().As<IValidator<User>>();
	        builder.RegisterType<ProjectValidator>().As<IValidator<Project>>();
	        builder.RegisterType<UserValidator>().As<IValidator<User>>();
	    }

	    private void SetupTools(ContainerBuilder builder)
		{
			builder.Register((x) => Messenger.Default).As<IMessenger>();
		}

        protected abstract IGeneralUnitOfWorkFactory GetInstanceOfIGeneralUnitOfWorkFactory(IComponentContext arg);

        private class AutofacValidatorFactory : ValidatorFactoryBase
        {
            private Func<IComponentContext> context;

            public AutofacValidatorFactory(Func<IComponentContext> context)
            {
                this.context = context;
            }

            protected override void TryResolve<T>(out IValidator<T> output)
            {
                context().TryResolve(out output);
            }
        }
    }
}
