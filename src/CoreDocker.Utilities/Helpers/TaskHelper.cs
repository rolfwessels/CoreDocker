using System;
using System.Threading.Tasks;
using Serilog;

namespace CoreDocker.Utilities.Helpers
{
    public static class TaskHelper
    {
        
        public static void ContinueWithNoWait<TType>(this Task<TType> updateAllReferences,
            Action<Task<TType>> logUpdate)
        {
            updateAllReferences.ContinueWith(logUpdate);
        }

        public static void ContinueWithAndLogError(this Task sendAsync, ILogger log)
        {
            sendAsync.ContinueWith(x =>
            {
                if (x.Exception != null)
                {
                    log.Error(x.Exception.Message);
                }
            });
        }
    }
}
