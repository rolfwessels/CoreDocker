using System.Reflection;

namespace CoreDocker.Sdk.SignalR
{
    public interface ICoreDockerSockets
    {
        ChatSocketClient Chat { get; }    
    }
}