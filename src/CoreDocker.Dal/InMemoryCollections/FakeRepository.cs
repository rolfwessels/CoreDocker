using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Persistence;

namespace CoreDocker.Dal.InMemoryCollections
{
    public class FakeRepository<T> : IRepository<T> where T : IBaseDalModel
    {
        public FakeRepository()
        {
            InternalDataList = new List<T>();
        }

        public List<T> InternalDataList { get; }

        private void AddAndSetUpdateDate(T entity)
        {
            InternalDataList.Add(entity.DynamicCastTo<T>());
            entity.UpdateDate = DateTime.Now;
        }

        public IQueryable<T> Query()
        {
            return InternalDataList.AsQueryable();
        }

        public Task<T> Add(T entity)
        {
            entity.CreateDate = DateTime.Now;
            if (entity is IBaseDalModelWithId baseDalModelWithId && string.IsNullOrEmpty(baseDalModelWithId.Id))
            {
                baseDalModelWithId.Id = BuildId();
            }

            AddAndSetUpdateDate(entity);
            return Task.FromResult(entity);
        }

        private static string BuildId()
        {
            return Guid.NewGuid().ToString("n").ToLower().Substring(0, 24);
        }


        public Task<IEnumerable<T>> AddRange(IEnumerable<T> entities)
        {
            var addRange = entities as T[] ?? entities.ToArray();
            foreach (var entity in addRange)
            {
                Add(entity);
            }

            return Task.FromResult(entities);
        }


        public Task<long> Update<TType>(Expression<Func<T, bool>> filter,
            Expression<Func<T, TType>> update,
            TType value)
            where TType : class
        {
            var enumerable = InternalDataList.Where(filter.Compile()).ToArray();
            foreach (var v in enumerable)
            {
                ReflectionHelper.ExpressionToAssign(v, update, value);
            }

            return Task.FromResult(enumerable.LongCount());
        }

        public Task<bool> Remove(Expression<Func<T, bool>> filter)
        {
            var array = InternalDataList.Where(filter.Compile()).ToArray();
            array.ForEach(x => InternalDataList.Remove(x));
            return Task.FromResult(array.Length > 0);
        }

        public async Task<List<T>> Find(Expression<Func<T, bool>> filter)
        {
            var fromResult = await FindInternal(filter);
            return fromResult.DynamicCastTo<List<T>>();
        }

        private Task<List<T>> FindInternal(Expression<Func<T, bool>> filter)
        {
            return Task.FromResult(InternalDataList.Where(filter.Compile()).ToList());
        }

        public async Task<T?> FindOne(Expression<Func<T, bool>> filter)
        {
            var list = await FindInternal(filter);
            return list.DynamicCastTo<List<T>>().FirstOrDefault();
        }

        public Task<long> Count()
        {
            return Task.FromResult(InternalDataList.LongCount());
        }

        public Task<long> Count(Expression<Func<T, bool>> filter)
        {
            return Task.FromResult(InternalDataList.Where(filter.Compile()).LongCount());
        }

        public async Task<long> UpdateMany(Expression<Func<T, bool>> filter, Action<IUpdateCalls<T>> upd)
        {
            var list = await FindInternal(filter);
            var updateCalls = new UpdateCalls<T>(list);
            upd(updateCalls);
            return list.LongCount();
        }

        public async Task<long> UpdateOne(Expression<Func<T, bool>> filter, Action<IUpdateCalls<T>> upd)
        {
            var list = (await FindInternal(filter)).Take(1).ToList();
            var updateCalls = new UpdateCalls<T>(list);
            upd(updateCalls);
            return list.LongCount();
        }

        public async Task<long> Upsert(Expression<Func<T, bool>> filter, Action<IUpdateCalls<T>> upd)
        {
            var list = (await FindInternal(filter)).Take(1).ToList();
            if (!list.Any())
            {
                var instance = Activator.CreateInstance<T>();
                InternalDataList.Add(instance);
                list.Add(instance);
            }

            var updateCalls = new UpdateCalls<T>(list);
            upd(updateCalls);
            return list.Count;
        }

        public async Task<T> FindOneAndUpdate(Expression<Func<T, bool>> filter,
            Action<IUpdateCalls<T>> upd,
            bool isUpsert = false)
        {
            var list = (await FindInternal(filter)).Take(1).ToList();
            if (isUpsert && !list.Any())
            {
                var instance = Activator.CreateInstance<T>();
                InternalDataList.Add(instance);
                list.Add(instance);
            }

            var updateCalls = new UpdateCalls<T>(list);
            upd(updateCalls);
            return list.First();
        }

        public class UpdateCalls<TClass> : IUpdateCalls<TClass>
        {
            private readonly List<TClass> _list;

            public UpdateCalls(List<TClass> list)
            {
                _list = list;
            }

            public IUpdateCalls<TClass> Set<TType>(Expression<Func<TClass, TType>> expression, TType value)
            {
                foreach (var item in _list)
                {
                    AssignNewValue(item, expression, value);
                }

                return this;
            }

            public IUpdateCalls<TClass> SetOnInsert<TT>(Expression<Func<TClass, TT>> expression, TT value)
            {
                foreach (var item in _list)
                {
                    AssignNewValue(item, expression, value);
                }

                return this;
            }

            public IUpdateCalls<TClass> Inc(Expression<Func<TClass, int>> expression, int value)
            {
                var compile = expression.Compile();
                foreach (var item in _list)
                {
                    var i = compile(item);
                    AssignNewValue(item, expression, i += value);
                }

                return this;
            }

            public IUpdateCalls<TClass> Inc(Expression<Func<TClass, long>> expression, long value)
            {
                var compile = expression.Compile();
                foreach (var item in _list)
                {
                    var i = compile(item);
                    AssignNewValue(item, expression, i += value);
                }

                return this;
            }

            public IUpdateCalls<TClass> Push<TT>(Expression<Func<TClass, IEnumerable<TT>>> expression, TT value)
            {
                throw new NotImplementedException();
            }


            public static void AssignNewValue<TObj, TValue>(TObj obj,
                Expression<Func<TObj, TValue>> expression,
                TValue value)
            {
                ReflectionHelper.ExpressionToAssign(obj, expression, value);
            }
        }

        public Task<T> Update(Expression<Func<T, bool>> filter, T entity)
        {
            Remove(filter);
            AddAndSetUpdateDate(entity);
            return Task.FromResult(entity);
        }

        public bool Remove(T entity)
        {
            if (entity is IBaseDalModelWithId baseDalModelWithId)
            {
                var baseDalModelWithIds =
                    InternalDataList.Cast<IBaseDalModelWithId>().FirstOrDefault(x => x.Id == baseDalModelWithId.Id);
                if (baseDalModelWithIds != null)
                {
                    InternalDataList.Remove((T)baseDalModelWithIds);
                    return true;
                }
            }

            return InternalDataList.Remove(entity);
        }

        public T Update(T entity, object t)
        {
            Remove(entity);
            AddAndSetUpdateDate(entity);
            return entity;
        }
    }
}