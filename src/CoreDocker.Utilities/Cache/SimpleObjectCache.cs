using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace CoreDocker.Utilities.Cache
{
    public class SimpleObjectCache : ISimpleObjectCache
    {
        private readonly TimeSpan _defaultCacheTime;
        private ConcurrentDictionary<string, CacheHolder> _objectCache;
        private DateTime _nextExpiry;

        public SimpleObjectCache(TimeSpan defaultCacheTime)
        {
            _defaultCacheTime = defaultCacheTime;
            _objectCache = new ConcurrentDictionary<string, CacheHolder>();
            _nextExpiry = DateTime.Now.Add(_defaultCacheTime);
        }

        #region ISimpleObjectCache Members

        public TValue Get<TValue>(string key, Func<TValue> getValue) where TValue : class
        {
            var retrievedValue = Get<TValue>(key);
            if (retrievedValue == null)
            {
                TValue result = getValue();
                if (result != null)
                {
                    Set(key, result);
                }
                return result;
            }
            return retrievedValue;
        }

        public TValue GetAndReset<TValue>(string key, Func<TValue> getValue) where TValue : class
        {
            CacheHolder values;
            if (_objectCache.TryGetValue(key, out values))
            {
                if (!values.IsExpired)
                {
                    return values.asValue<TValue>();
                }
            }
            return Set(key, getValue());
        }

        public TValue Set<TValue>(string key, TValue newvalue)
        {
            var cacheHolder = new CacheHolder(newvalue, DateTimeOffset.Now.Add(_defaultCacheTime));
            _objectCache.AddOrUpdate(key, s => cacheHolder, (s, holder) => cacheHolder);
            return newvalue;
        }

        public TValue Get<TValue>(string key) where TValue : class
        {
            CacheHolder values;
            if (_objectCache.TryGetValue(key, out values))
            {
                if (!values.IsExpired)
                {
                    return values.asValue<TValue>();
                }
                else
                {
                    StartCleanup();
                }
            }
            return null;
        }

        #endregion

        #region Private Methods

        private void StartCleanup()
        {
            if (DateTime.Now > _nextExpiry)
            {
                lock (_objectCache)
                {
                    if (DateTime.Now > _nextExpiry)
                    {
                        _nextExpiry = DateTime.Now.Add(_defaultCacheTime);
                        Task.Run(() =>
                        {
                            foreach (var cacheHolder in _objectCache.ToArray())
                            {
                                if (cacheHolder.Value.IsExpired)
                                {
                                    CacheHolder ss;
                                    _objectCache.TryRemove(cacheHolder.Key, out ss);
                                }
                            }
                        });
                    }
                }
            }
        }

        private class CacheHolder
        {
            private DateTimeOffset _expire;
            private object _value;

            public CacheHolder(object value, DateTimeOffset expire)
            {
                _value = value;
                _expire = expire;
            }

            public bool IsExpired
            {
                get { return DateTime.Now > _expire; }
            }

            internal TValue asValue<TValue>() where TValue : class
            {
                return _value as TValue;
            }
        }

        #endregion
    }
}