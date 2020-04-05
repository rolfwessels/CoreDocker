using System;
using System.Threading.Tasks;
using CoreDocker.Shared;
using CoreDocker.Utilities.Helpers;
using Microsoft.AspNetCore.SignalR.Client;

namespace CoreDocker.Sdk.SignalR
{
    public class SocketClientBase
    {
        private readonly Lazy<HubConnection> _connectionBuilder;
        private readonly CoreDockerSockets _sockets;
        private bool _isStarted;

        protected SocketClientBase(CoreDockerSockets sockets)
        {
            _sockets = sockets;
            _connectionBuilder = new Lazy<HubConnection>(Connect);
        }

        protected HubConnection Connect()
        {
            var uri = new Uri(_sockets.BaseUrl.UriCombine(SignalRHubUrls.ChatUrl));
            var hubConnectionBuilder = new HubConnectionBuilder().WithUrl(uri);
            if (_sockets.OverrideLogging != null)
                hubConnectionBuilder = hubConnectionBuilder.ConfigureLogging(_sockets.OverrideLogging);
            var hubConnection = hubConnectionBuilder.Build();
            return hubConnection;
        }

        protected async Task<HubConnection> EnsureStarted()
        {
            if (!_isStarted)
            {
                await _connectionBuilder.Value.StartAsync();
                _isStarted = true;
            }

            return _connectionBuilder.Value;
        }
    }
}