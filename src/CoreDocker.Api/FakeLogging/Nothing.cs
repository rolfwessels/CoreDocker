using System;

namespace log4net
{
    internal class Nothing : ILog
    {
        #region ILog Members

        public void Error(string error)
        {
        }

        public void Error(string message, Exception exception)
        {
        }

        public void Warn(string v)
        {
        }

        #endregion
    }

    public class LogManager
    {
        internal static ILog GetLogger<T>()
        {
            return new Nothing();
        }
    }

    public interface ILog
    {
        void Error(string error);
        void Error(string message, Exception exception);
        void Warn(string v);
    }
}