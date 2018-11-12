using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreDocker.Core.Framework.BaseManagers
{
    public interface IBaseManager<T>
    {
        Task<List<T>> Get();
        Task<List<T>> Get(Expression<Func<T, bool>> filter);
        Task<T> GetById(string id);
        Task<IQueryable<T>>  Query(Func<IQueryable<T>, IQueryable<T>> query);
    }
}