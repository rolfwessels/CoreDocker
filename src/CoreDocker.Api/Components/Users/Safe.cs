using System;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Utilities.Helpers;
using FluentValidation;
using GraphQL;
using GraphQL.Types;
using log4net;

namespace CoreDocker.Api.Components.Users
{
    public class Safe
    {
        private readonly ILog _log;

        public Safe(ILog log)
        {
            _log = log;
        }


        public Func<ResolveFieldContext<object>, object> Wrap(Func<ResolveFieldContext<object>, object> func)
        {
            return context =>
            {
                try
                {
                    var update = func(context);
                    if (update is Task task) task.Wait();

                    return update;
                }
                catch (AggregateException e) when (e.InnerExceptions.OfType<ValidationException>().Any())
                {
                    var validationExceptions = e.InnerExceptions.OfType<ValidationException>().ToArray();
                    if (validationExceptions.Any())
                    {
                        validationExceptions.ForEach(exception => LogAndThrowValidation(exception, context));
                        return null;
                    }

                    throw;
                }
                catch (AggregateException e) when (e.InnerExceptions.OfType<ReferenceException>().Any())
                {
                    var validationExceptions = e.InnerExceptions.OfType<ReferenceException>().ToArray();
                    if (validationExceptions.Any())
                    {
                        validationExceptions.ForEach(e1 => LogAndThrowValidation(e1, context));
                        return null;
                    }

                    throw;
                }

                catch (ReferenceException e)
                {
                    LogAndThrowValidation(e, context);
                    return null;
                }

                catch (ValidationException e)
                {
                    LogAndThrowValidation(e, context);
                    return null;
                }
                catch (Exception e)
                {
                    _log.Error(e.Message, e);
                    throw;
                }
            };
        }

        #region Private Methods

        private void LogAndThrowValidation(ValidationException e, ResolveFieldContext<object> context)
        {
            _log.Warn(e.Message, e);
            context.Errors.AddRange(e.Errors.Select(x => new ExecutionError(x.ErrorMessage) {Code = "VALIDATION", Path = context.Path} ));
//            throw new ExecutionError(e.Errors.Select(x => x.ErrorMessage).FirstOrDefault(), e)
//            {
//                Code = "VALIDATION"
//            };
        }

        private void LogAndThrowAggregateValidation(ValidationException e, ResolveFieldContext<object> context)
        {
            _log.Warn(e.Message, e);
            var errors = e.Errors.Select(err => new ExecutionError(err.ErrorMessage, e)
            {
                Code = "VALIDATION"
            });
            throw new AggregateException(errors);
        }

        private void LogAndThrowValidation(ReferenceException e, ResolveFieldContext<object> context)
        {
            _log.Warn(e.Message, e);
            throw new ExecutionError(e.Message, e)
            {
                Code = "VALIDATION"
            };
        }

        #endregion
    }
}