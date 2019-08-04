﻿using System;
using System.Linq;
using System.Reflection;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Utilities.Helpers;
using FluentValidation;
using HotChocolate;
using Serilog;

namespace CoreDocker.Api.GraphQl
{

    public class ErrorFilter : IErrorFilter
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod().DeclaringType);

        #region Implementation of IErrorFilter

        public IError OnError(IError error)
        {

            if (error.Exception is AggregateException aggregateException)
            {
                var validationExceptions = aggregateException.InnerExceptions.OfType<ValidationException>().ToArray();
                if (validationExceptions.Any())
                    return LogAndThrowValidation(error, validationExceptions.FirstOrDefault());
                var referenceExceptions = aggregateException.InnerExceptions.OfType<ReferenceException>().ToArray();
                if (referenceExceptions.Any())
                    return LogAndThrowValidation(error);
            }

            if (error.Exception is ReferenceException)
            {
                return LogAndThrowValidation(error);
            }

            if (error.Exception is ValidationException exception)
            {
                return LogAndThrowValidation(error, exception);
            }
            
            _log.Error(BuildMessage(error), error.Exception);
            return error.WithCode(error.Exception?.GetType().Name.Replace("Exception","")?? error.Code);
        }

        private static string BuildMessage(IError error)
        {
            return $"{error.Path.StringJoin(">")}:{error.Exception?.Message ?? error.Message}";
        }

        private IError LogAndThrowValidation(IError error, ValidationException validationException = null)
        {
            _log.Warning(BuildMessage(error), error.Exception);
            return error.WithMessage(validationException?.Errors.First().ErrorMessage ?? error.Exception.Message)
                .WithCode("Validation");
        }
        
        #endregion
    }

}
