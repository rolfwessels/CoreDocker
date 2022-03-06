using System;
using MediatR;

namespace CoreDocker.Core.Framework.CommandQuery
{
    public abstract record CommandRequestBase : IRequest<CommandResult>, ICommandProperties
    {
        protected CommandRequestBase(string id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            CorrelationId = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now.ToUniversalTime();
        }

        public override string ToString()
        {
            return $"{GetType().Name}_{CorrelationId}";
        }

        #region Implementation of ICommandProperties

        public string CorrelationId { get; set; }
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }

        #endregion
    }
}