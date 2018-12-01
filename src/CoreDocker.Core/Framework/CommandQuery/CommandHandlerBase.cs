using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.Mappers;
using log4net;
using MediatR;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public abstract class CommandHandlerBase<T> : IRequestHandler<T, CommandResult> where T : CommandRequestBase
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region IRequestHandler<T,CommandResult> Members

        public async Task<CommandResult> Handle(T request, CancellationToken cancellationToken)
        {
            using (LogicalThreadContext.Stacks["NDC"].Push($"Topic:{request}|"))
            {
                try
                {
                    await ProcessCommand(request);
                    return request.ToCommandResult();
                }
                catch (Exception e)
                {
                    _log.Error($"CommandHandlerBase:Handle {e.Message}");
                    throw;
                }
            }
        }

        #endregion

        public abstract Task ProcessCommand(T request);
    }
}