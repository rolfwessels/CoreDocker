using System;

namespace CoreDocker.Core.Framework.BaseManagers
{
    public class ReferenceException : Exception
    {
        public ReferenceException(string message) : base(message)
        {
        }
    }
}