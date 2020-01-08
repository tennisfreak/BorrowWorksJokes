using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jokes.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task<T> GetOne<A>(A id);
        IEnumerable<T> Get();
        Task<Boolean> Save(T item);
        Task<T> Add(T item);
    }
}
