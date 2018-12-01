using CoreDocker.Api.SignalR.Hubs;
using CoreDocker.Shared;
using Microsoft.AspNetCore.Builder;

namespace CoreDocker.Api.SignalR
{
    public static class SingalRSetup
    {
        public static void UseSingalRSetup(this IApplicationBuilder app)
        {
            app.UseSignalR(routes => { routes.MapHub<ChatHub>(SignalRHubUrls.ChatUrl); });
        }
    }
}