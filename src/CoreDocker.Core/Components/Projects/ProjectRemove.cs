﻿using System;
using System.Threading;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.CommandQuery;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Persistence;

namespace CoreDocker.Core.Components.Projects
{
    public class ProjectRemove
    {
        #region Nested type: Handler

        public class Handler : CommandHandlerBase<Request>
        {
            private readonly ICommander _commander;
            private readonly IGeneralUnitOfWorkFactory _persistence;

            public Handler(IGeneralUnitOfWorkFactory persistence,
                ICommander commander)
            {
                _persistence = persistence;
                _commander = commander;
            }

            #region Overrides of CommandHandlerBase<Request>

            public override async Task ProcessCommand(Request request, CancellationToken cancellationToken)
            {
                using var connection = _persistence.GetConnection();
                var foundProject = await connection.Projects.FindOrThrow(request.Id);
                await connection.Projects.Remove(x => x.Id == foundProject.Id);
                await _commander.Notify(request.ToEvent(), cancellationToken);
            }

            #endregion
        }

        #endregion

        #region Nested type: Notification

        public class Notification : CommandNotificationBase
        {
            #region Overrides of CommandNotificationBase

            public override string EventName => "ProjectRemoved";

            #endregion

            public bool WasRemoved { get; set; }
        }

        #endregion

        #region Nested type: Request

        public record Request(string Id) : CommandRequestBase(Id);

        #endregion
    }
}