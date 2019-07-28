using System.Threading.Tasks;
using CoreDocker.Shared;
using CoreDocker.Shared.Interfaces.Sockets;
using Microsoft.AspNetCore.SignalR;

namespace CoreDocker.Api.SignalR.Hubs
{
    public class ChatHub : Hub, IChatHub
    {
        #region IChatHub Members

        public async Task Send(string message)
        {
            await Clients.All.SendAsync(SignalRHubUrls.ChatUrlReceiveCommand, message);
        }

        #endregion
    }
}
