using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreDocker.Utilities.Helpers;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Auth;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.Users;
using CoreDocker.Dal.Persistance;

namespace CoreDocker.Dal.InMemoryCollections
{
    public class InMemoryGeneralUnitOfWork : IGeneralUnitOfWork
    {
        public InMemoryGeneralUnitOfWork()
        {
            Users = new FakeRepository<User>();
            Applications = new FakeRepository<Application>();
            Projects = new FakeRepository<Project>();
            UserGrants = new FakeRepository<UserGrant>();
        }


        #region Implementation of IDisposable

        public void Dispose()
        {
        }

        #endregion

        #region Implementation of IGeneralUnitOfWork

        public IRepository<User> Users { get; private set; }
        public IRepository<Application> Applications { get; private set; }
        public IRepository<Project> Projects { get; private set; }
        public IRepository<UserGrant> UserGrants { get; private set; }

        #endregion

        #region Nested type: FakeRepository

        public class FakeRepository<T> : IRepository<T> where T : IBaseDalModel
        {
            private readonly List<T> _list;

            public FakeRepository()
            {
                _list = new List<T>();
            }

            #region Implementation of IRepository<T>

            public IQueryable<T> Query()
            {
                return _list.AsQueryable();
            }

            public Task<T> Add(T entity)
            {
                entity.CreateDate = DateTime.Now;
                var baseDalModelWithId = entity as IBaseDalModelWithId;
                if (baseDalModelWithId != null) baseDalModelWithId.Id = Guid.NewGuid().ToString("n");
                AddAndSetUpdateDate(entity);
                return Task.FromResult(entity);
            }


            public Task<IEnumerable<T>> AddRange(IEnumerable<T> entities)
            {
                var addRange = entities as T[] ?? entities.ToArray();
                foreach (T entity in addRange)
                {
                    Add(entity);
                }
                return Task.FromResult(entities);
            }


            public Task<long> Update<TType>(Expression<Func<T, bool>> filter, Expression<Func<T, TType>> update, TType value) where TType : class
            {
                var enumerable = _list.Where(filter.Compile()).ToArray();
                foreach (var v in enumerable)
                {
                    var type = update.Compile()(v);
                    PropertyCopy.Copy<TType, TType>(value, type);
                }
                return Task.FromResult(enumerable.LongCount());
            }

            public Task<bool> Remove(Expression<Func<T, bool>> filter)
            {
                T[] array = _list.Where(filter.Compile()).ToArray();
                array.ForEach(x => _list.Remove(x));
                return Task.FromResult(array.Length > 0);
            }

            public Task<List<T>> Find(Expression<Func<T, bool>> filter)
            {
                return Task.FromResult(_list.Where(filter.Compile()).ToList());
            }

            public Task<T> FindOne(Expression<Func<T, bool>> filter)
            {
                var predicate = filter.Compile();
                return Task.FromResult(_list.FirstOrDefault((x) =>
                {
                    try
                    {
                        return predicate(x);
                    }
                    catch (NullReferenceException)
                    {
                        return false;
                    }
                }));
            }

            public Task<long> Count()
            {
                return Task.FromResult(_list.LongCount());
            }

            public Task<long> Count(Expression<Func<T, bool>> filter)
            {
                return Task.FromResult(_list.Where(filter.Compile()).LongCount());
            }

            public Task<T> Update(Expression<Func<T, bool>> filter, T entity)
            {
                Remove(filter);
                AddAndSetUpdateDate(entity);
                return Task.FromResult(entity);
            }

            public bool Remove(T entity)
            {
                var baseDalModelWithId = entity as IBaseDalModelWithId;
                if (baseDalModelWithId != null)
                {
                    IBaseDalModelWithId baseDalModelWithIds =
                        _list.Cast<IBaseDalModelWithId>().FirstOrDefault(x => x.Id == baseDalModelWithId.Id);
                    if (baseDalModelWithIds != null)
                    {
                        _list.Remove((T) baseDalModelWithIds);
                        return true;
                    }
                }
                return _list.Remove(entity);
            }

            public T Update(T entity, object t)
            {
                Remove(entity);
                AddAndSetUpdateDate(entity);
                return entity;
            }

            #endregion

            #region Private Methods

            private void AddAndSetUpdateDate(T entity)
            {
                _list.Add(entity);
                entity.UpdateDate = DateTime.Now;
            }

            #endregion
        }

        #endregion
    }
}
