using System;
using Serilog;

namespace CoreDocker.Core.Framework.Logging
{
    public static class LoggingHelper
    {
        public const int MB = 1048576;
        private static bool _hasValue;
        private static readonly object _locker = new();

        public static ILogger SetupOnce(Func<ILogger> func)
        {
            if (!_hasValue)
            {
                lock (_locker)
                {
                    if (!_hasValue)
                    {
                        _hasValue = true;
                        return Log.Logger = func();
                    }
                }
            }

            return Log.Logger;
        }
    }
}