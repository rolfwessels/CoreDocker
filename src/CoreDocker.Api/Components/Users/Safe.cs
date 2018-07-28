using System;
using System.Linq;
using System.Threading.Tasks;
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

        void LogAndThrowValidation(ValidationException e, ResolveFieldContext<object> context)
        {
            _log.Warn(e.Message, e);
            throw new ExecutionError(e.Errors.Dump("1").Select(x=>x.ErrorMessage).FirstOrDefault(), e) {Code = "VALIDATION" };
        }

        void LogAndThrowAggregateValidation(ValidationException e, ResolveFieldContext<object> context)
        {
            _log.Warn(e.Message, e);
            var errors = e.Errors.Select(err => new ExecutionError(err.ErrorMessage, e)
            {
                Code = "VALIDATION",
            });
            throw new AggregateException(errors);
        }

        public Func<ResolveFieldContext<object>, object> Wrap(Func<ResolveFieldContext<object>, object> func)
        {

            return context => {
                try
                {
                    var update = func(context);
                    if (update is Task task)
                    {
                        task.Wait();
                    }

                    return update;
                }
                catch (AggregateException e)
                {
                    var validationExceptions = e.InnerExceptions.OfType<ValidationException>().ToArray();
                    if (validationExceptions.Any())
                    {
                        validationExceptions.ForEach(e1 => LogAndThrowValidation(e1, context));
                        return null;
                    }
                    throw ;
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

        

    }
}