﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;
using StackExchange.Redis;

namespace CoreDocker.Core.Framework.MessageUtil
{
    public class RedisMessenger : IMessenger
    {
        private static readonly ILogger _log = Log.ForContext(MethodBase.GetCurrentMethod()!.DeclaringType);

        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<WeakReference, object>> _dictionary = new();
        private readonly string _redisHost;

        private readonly Lazy<ISubscriber> _sub;

        public RedisMessenger(string redisHost)
        {
            _redisHost = redisHost;
            _sub = new Lazy<ISubscriber>(GetSubscriber);
        }

        public void Clean()
        {
            UnRegister(new object());
        }

        public int Count()
        {
            return _dictionary.Sum(x => x.Value.Count);
        }

        private ISubscriber GetSubscriber()
        {
            var redis = ConnectionMultiplexer.Connect(_redisHost);
            return redis.GetSubscriber();
        }

        public async Task Send<T>(T value)
        {
            _log.Debug($"RedisMessenger:Send {typeof(T).Name}:{Serialize(value)}");
            await _sub.Value.PublishAsync(typeof(T).Name, Serialize(value));
        }

        public void Register<T>(object receiver, Action<T> action) where T : class
        {
            Register(typeof(T), receiver, o => action((T)o));
        }

        public void Register(Type type, object receiver, Action<object> callBackToClient)
        {
            var redisChannel = type.Name;
            var references = _dictionary.GetOrAdd(type, _ => new ConcurrentDictionary<WeakReference, object>());
            var handler = references
                .GetOrAdd(new WeakReference(receiver), _ =>
                    {
                        void Action(RedisChannel channel, RedisValue message)
                        {
                            var deserializeObject = JsonSerializer.Deserialize(message, type)!;
                            _log.Debug($"RedisMessenger:Received {deserializeObject.GetType().Name}:{message}");
                            callBackToClient(deserializeObject);
                        }

                        return (Action<RedisChannel, RedisValue>)Action;
                    }
                ) as Action<RedisChannel, RedisValue>;


            _sub.Value.Subscribe(redisChannel, handler);
        }


        public void UnRegister<T>(object receiver)
        {
            UnRegister(typeof(T), receiver);
        }

        public void UnRegister(Type type, object receiver)
        {
            if (_dictionary.TryGetValue(type, out var typeFound))
            {
                foreach (var key in typeFound.Keys)
                {
                    Remove(receiver, key, typeFound, type);
                }
            }
        }


        public void UnRegister(object receiver)
        {
            foreach (var keyValuePair in _dictionary)
            foreach (var key in keyValuePair.Value.Keys)
            {
                Remove(receiver, key, keyValuePair.Value, keyValuePair.Key);
            }
        }

        private void Remove(object receiver,
            WeakReference key,
            ConcurrentDictionary<WeakReference, object> typeFound,
            Type type)
        {
            if (!key.IsAlive)
            {
                Unsubscribe(key, typeFound, type);
                return;
            }

            if (key.Target == receiver)
            {
                Unsubscribe(key, typeFound, type);
            }
        }

        private void Unsubscribe(WeakReference key, ConcurrentDictionary<WeakReference, object> typeFound, Type type)
        {
            if (typeFound.TryRemove(key, out var handler))
            {
                _log.Debug($"RedisMessenger:Unsubscribe {type.Name}");
                _sub.Value.Unsubscribe(type.Name, handler as Action<RedisChannel, RedisValue>);
            }
        }

        private RedisValue Serialize<T>(T value)
        {
            return new RedisValue(JsonSerializer.Serialize(value));
        }
    }
}