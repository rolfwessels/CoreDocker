using System;

namespace CoreDocker.Dal.Persistence
{
    public class ReferenceException : Exception
    {
        public ReferenceException(string message) : base(message)
        {
        }
    }
}
