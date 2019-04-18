using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WeatherInsurance.Integration.Database
{
    public interface IRepository<T> where T : class
    {
        T Get(params object[] keys);

        IEnumerable<T> Get();

        IEnumerable<T> Find(Expression<Func<T, bool>> query);

        int Update(T entity);

        long AddNew(T entity);

        int Delete(params object[] keys);
    }
}
