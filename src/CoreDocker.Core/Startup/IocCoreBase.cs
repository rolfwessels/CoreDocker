using System;
using System.Reflection;
using Autofac;
using CoreDocker.Core.Components.Projects;
using CoreDocker.Core.Components.Users;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Framework.MessageUtil;
using FluentValidation;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistance;
using log4net;
using UserGrant = CoreDocker.Dal.Models.Users.UserGrant;
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
	            _log.Error("IocCoreBase:Delegate " + e.Message, e);
	            _log.Error(e.Source);
	            _log.Error(e.StackTrace);
	            throw;
	        }
	    }

	    private static void SetupManagers(ContainerBuilder builder)
		{
            builder.RegisterType<BaseManagerArguments>();
            builder.RegisterType<ProjectManager>().As<IProjectManager>();
            builder.RegisterType<RoleManager>().As<IRoleManager>();
            builder.RegisterType<UserManager>().As<IUserManager>();
            builder.RegisterType<UserGrantManager>().As<IUserGrantManager>();
		}

	    private static void SetupValidation(ContainerBuilder builder)
	    {
            builder.RegisterType<AutofacValidatorFactory>().As<Dal.Validation.IValidatorFactory>();
	        builder.RegisterType<UserValidator>().As<IValidator<User>>();
	        builder.RegisterType<ProjectValidator>().As<IValidator<Project>>();
	        builder.RegisterType<UserGrantValidator>().As<IValidator<UserGrant>>();
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
