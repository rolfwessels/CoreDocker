using System;

namespace CoreDocker.Utilities.Cache
{
    public interface ISimpleObjectCache
    {
        TValue Get<TValue>(string key, Func<TValue> getValue) where TValue : class;
        TValue GetAndReset<TValue>(string key, Func<TValue> getValue) where TValue : class;
        TValue Set<TValue>(string value, TValue newvalue);
        TValue Get<TValue>(string key) where TValue : class;
    }
}