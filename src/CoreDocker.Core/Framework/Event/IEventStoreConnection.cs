using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDocker.Core.Framework.Event
{
    public interface IEventStoreConnection
    {
        Task Append<T>(T value, CancellationToken token);
        IAsyncEnumerable<EventHolder> Read(CancellationToken token);
        void Register<T>();
        Task RemoveSteam(string streamName);
        IObservable<EventHolder> ReadAndFollow(CancellationToken token);
    }
}