using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.Mappers;
using MediatR;
using Serilog;
using Serilog.Context;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public abstract class CommandHandlerBase<T> : IRequestHandler<T, CommandResult> where T : CommandRequestBase
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()?.DeclaringType);

        public async Task<CommandResult> Handle(T request, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("Topic", request))
            {
                try
                {
                    await ProcessCommand(request, cancellationToken);
                    return request.ToCommandResult();
                }
                catch (Exception e)
                {
                    _log.Error($"CommandHandlerBase:Handle {e.Message}");
                    throw;
                }
            }
        }

        public abstract Task ProcessCommand(T request, CancellationToken cancellationToken);
    }
}