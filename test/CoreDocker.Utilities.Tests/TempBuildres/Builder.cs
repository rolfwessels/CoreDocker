using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace FizzWare.NBuilder
{
    public class Builder<T>
    {
        public static T CreateNew()
        {
            return Activator.CreateInstance<T>();
        }

        public static IList<T> CreateListOfSize(int size)
        {
            return Enumerable.Range(0, size).Select(x => CreateNew()).ToList();
        }


    }
}

