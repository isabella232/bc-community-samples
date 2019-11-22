using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WeatherInsurance.Integration.Database
{
    public interface IRepository<T> where T : class
    {
        Task<T> Get(params object[] keys);

        Task<IEnumerable<T>> Get();

        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> query);

        Task<int> Update(T entity);

        Task<long> AddNew(T entity);

        Task<int> Delete(params object[] keys);
    }
}
