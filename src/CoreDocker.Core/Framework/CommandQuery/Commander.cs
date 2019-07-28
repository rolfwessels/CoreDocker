using System.Threading.Tasks;
using CoreDocker.Dal.Persistence;
using MediatR;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public class Commander : ICommander
    {
        private readonly IGeneralUnitOfWorkFactory _factory;
        private readonly IMediator _mediator;

        public Commander(IGeneralUnitOfWorkFactory factory, IMediator mediator)
        {
            _factory = factory;
            _mediator = mediator;
        }

        #region Implementation of ICommander

        public string NewId => _factory.NewId;

        public async Task SendEvent<T>(T @event) where T : CommandNotificationBase
        {
            await _mediator.Publish(@event);
        }

        public async Task<CommandResult> Execute<T>(T @from) where T : CommandRequestBase
        {
            return await _mediator.Send(@from);
        }

        #endregion
    }
}
