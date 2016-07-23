using System;

namespace FizzWare.NBuilder
{
    public class Builder<T>
    {
        public static T CreateNew()
        {
            return Activator.CreateInstance<T>();
        }

        public static object CreateListOfSize(int size)
        {
            throw new NotImplementedException();
        }
    }
}

