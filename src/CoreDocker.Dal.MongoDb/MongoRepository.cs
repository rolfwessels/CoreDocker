using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Persistance;
using CoreDocker.Dal.Persistence;
using MongoDB.Driver;

namespace CoreDocker.Dal.MongoDb
{
    public class MongoRepository<T> : IRepository<T> where T : IBaseDalModel
    {
        public MongoRepository(IMongoDatabase database)
        {
            Collection = database.GetCollection<T>(typeof(T).Name);
        }

        public IMongoCollection<T> Collection { get; }

        public Task<List<T>> Find()
        {
            return Find(x => true);
        }

        #region Implementation of IRepository<T>

        public IQueryable<T> Query()
        {
            return Collection.AsQueryable();
        }

        public async Task<T> Add(T entity)
        {
            entity.CreateDate = DateTime.Now;
            entity.UpdateDate = DateTime.Now;
            await Collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<T>> AddRange(IEnumerable<T> entities)
        {
            var enumerable = entities as IList<T> ?? entities.ToList();
            foreach (var entity in enumerable)
            {
                entity.CreateDate = DateTime.Now;
                entity.UpdateDate = DateTime.Now;
            }

            await Collection.InsertManyAsync(enumerable);

            return enumerable;
        }


        public async Task<T> Update(Expression<Func<T, bool>> filter, T entity)
        {
            entity.UpdateDate = DateTime.Now;
            await Collection.ReplaceOneAsync(Builders<T>.Filter.Where(filter), entity);
            return entity;
        }

        public async Task<long> Update<TType>(Expression<Func<T, bool>> filter, Expression<Func<T, TType>> update,
            TType value) where TType : class
        {
            var filterDefinition = Builders<T>.Filter.Where(filter);
            var updateDefinition = Builders<T>.Update
                .Set(update, value)
                .CurrentDate(x => x.UpdateDate);

            var result = await Collection.UpdateManyAsync(filterDefinition, updateDefinition);
            return result.ModifiedCount;
        }

        public async Task<bool> Remove(Expression<Func<T, bool>> filter)
        {
            var deleteResult = await Collection.DeleteOneAsync(filter);
            return deleteResult.DeletedCount > 0;
        }

        public Task<List<T>> Find(Expression<Func<T, bool>> filter)
        {
            return Collection.Find(Builders<T>.Filter.Where(filter)).ToListAsync();
        }

        public Task<T> FindOne(Expression<Func<T, bool>> filter)
        {
            return Collection.Find(Builders<T>.Filter.Where(filter)).FirstOrDefaultAsync();
        }


        public Task<long> Count()
        {
            return Count(x => true);
        }

        public Task<long> Count(Expression<Func<T, bool>> filter)
        {
            return Collection.CountDocumentsAsync(Builders<T>.Filter.Where(filter));
        }

        #endregion
    }
}