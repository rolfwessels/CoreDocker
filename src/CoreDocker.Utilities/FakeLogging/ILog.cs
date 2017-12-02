using System;
using Microsoft.Extensions.Logging;

namespace CoreDocker.Utilities.FakeLogging
{
    public interface ILog
    {
        ILogger Logger { get; }
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Warn(string message, Exception exception);
        void Error(string error);
        void Error(string message, Exception exception);
    }
}