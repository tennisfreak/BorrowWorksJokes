using Jokes.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jokes.Interfaces
{
    public interface IJokesRepo : IBaseRepository<Joke>
    {
        PaginatedList<Joke> SearchJokes(string typeFilters, string textFilters, int pageNumber, int pageSize);
        Task<bool> DeleteJoke(Guid id);
        Task<List<JokeType>> GetJokeTypes(bool onlyActive);
        Task<Joke> GetRandomJoke();
    }
}
