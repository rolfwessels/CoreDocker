using System;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Sdk.SignalR
{
    public class CoreDockerSockets : ICoreDockerSockets
    {
        public CoreDockerSockets(string baseUrl)
        {
            BaseUrl = baseUrl;
            Chat = new ChatSocketClient(this);
        }

        public string BaseUrl { get; }
        public Action<ILoggingBuilder> OverrideLogging { get; set; }

        #region Implementation of ICoreDockerSockets

        public ChatSocketClient Chat { get; }

        #endregion
    }
}
