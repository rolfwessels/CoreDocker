using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Api.WebApi.Exceptions;
using CoreDocker.Shared.Models.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace CoreDocker.Api.WebApi.Filters
{
    public class CaptureExceptionFilter : ExceptionFilterAttribute
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        public override Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception.ToFirstExceptionOfException();

            if (exception is ApiException apiException)
            {
                RespondWithTheExceptionMessage(context, apiException);
            }
            else if (IsSomeSortOfValidationError(exception))
            {
                RespondWithBadRequest(context, exception);
            }
            else if (exception is ValidationException validationException)
            {
                RespondWithValidationRequest(context, validationException);
            }
            else
            {
                RespondWithInternalServerException(context, exception);
            }

            return base.OnExceptionAsync(context);
        }

        public bool IsSomeSortOfValidationError(Exception exception)
        {
            return exception is System.ComponentModel.DataAnnotations.ValidationException ||
                   exception is ArgumentException;
        }

        private void RespondWithTheExceptionMessage(ExceptionContext context, ApiException exception)
        {
            var errorMessage = new ErrorMessage(exception.Message);
            context.Result = CreateResponse(exception.HttpStatusCode, errorMessage);
        }

        private void RespondWithBadRequest(ExceptionContext context, Exception exception)
        {
            var errorMessage = new ErrorMessage(exception.Message);
            context.Result = CreateResponse(HttpStatusCode.BadRequest, errorMessage);
        }

        private void RespondWithValidationRequest(ExceptionContext context,
            ValidationException validationException)
        {
            var errorMessage =
                new ErrorMessage(validationException.Errors.Select(x => x.ErrorMessage).FirstOrDefault() ??
                                 "Validation Error");
            context.Result = CreateResponse(HttpStatusCode.BadRequest, errorMessage);
        }

        private void RespondWithInternalServerException(ExceptionContext context, Exception exception)
        {
            const HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            var errorMessage =
                new ErrorMessage("An internal system error has occurred. The developers have been notified.");
            _log.Error(exception.Message, exception);
#if DEBUG
            errorMessage.AdditionalDetail = exception.Message;
#endif
            context.Result = CreateResponse(httpStatusCode, errorMessage);
        }

        private IActionResult CreateResponse(HttpStatusCode httpStatusCode, object errorMessage)
        {
            return new ObjectResult(errorMessage) { StatusCode = (int)httpStatusCode };
        }
    }
}