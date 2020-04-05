using System;
using System.Threading.Tasks;
using CoreDocker.Shared;
using CoreDocker.Shared.Interfaces.Sockets;
using Microsoft.AspNetCore.SignalR.Client;

namespace CoreDocker.Sdk.SignalR
{
    public class ChatSocketClient : SocketClientBase, IChatHub
    {
        public ChatSocketClient(CoreDockerSockets sockets) : base(sockets)
        {
        }

        #region IChatHub Members

        #region Implementation of IChatHub

        public async Task Send(string message)
        {
            var hubConnection = await EnsureStarted();
            await hubConnection.SendCoreAsync(SignalRHubUrls.ChatUrlSendCommand, new object[] {message});
        }

        #endregion

        #endregion

        public async Task OnReceived(Action<string> action)
        {
            var hubConnection = await EnsureStarted();
            hubConnection.On(SignalRHubUrls.ChatUrlReceiveCommand, action);
        }
    }
}