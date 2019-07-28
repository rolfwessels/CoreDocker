using System.Threading.Tasks;

namespace CoreDocker.Shared.Interfaces.Sockets
{
    public interface IChatHub
    {
        Task Send(string message);
    }
}
