using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.Mappers;
using Serilog;
using MediatR;
using Serilog.Context;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public abstract class CommandHandlerBase<T> : IRequestHandler<T, CommandResult> where T : CommandRequestBase
    {

        #region IRequestHandler<T,CommandResult> Members

        public async Task<CommandResult> Handle(T request, CancellationToken cancellationToken)
        {
            using (LogContext.PushProperty("Topic", request))
            {
                try
                {
                    await ProcessCommand(request);
                    return request.ToCommandResult();
                }
                catch (Exception e)
                {
                    Log.Error($"CommandHandlerBase:Handle {e.Message}");
                    throw;
                }
            }
        }

        #endregion

        public abstract Task ProcessCommand(T request);
    }
}
