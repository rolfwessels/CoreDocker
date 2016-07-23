using System;

namespace FizzWare.NBuilder
{
    public class Builder<T>
    {
        internal static T CreateNew()
        {
            return Activator.CreateInstance<T>();
        }
    }
}

