using Jokes.Entities;
using Jokes.Interfaces;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Jokes.Repository
{
    public class JokesRepo : BaseRepository, IJokesRepo
    {
        public JokesRepo(RepoFactory factory) : base(factory) { }

        public async Task<Joke> Add(Joke item)
        {
            return await base.Add<Joke>(item);
        }

        public IEnumerable<Joke> Get()
        {
            return base.Get<Joke>();
        }

        public async Task<Joke> GetOne<Guid>(Guid id)
        {
            return await base.GetOne<Joke, Guid>(id);
        }

        public async Task<bool> Save(Joke item)
        {
            return await base.Save<Joke, Guid>(item, item.ID);
        }

        public async Task<bool> DeleteJoke(Guid id)
        {
            return await base.Delete<Joke>(id);
        }

        public PaginatedList<Joke> SearchJokes(string typeFilters, string textFilters, int pageNumber, int pageSize)
        {
            var query = from q in _context.Jokes
                        select q;

            Expression<Func<Joke, bool>> expression = null;

            if (!String.IsNullOrWhiteSpace(typeFilters))
                {
                foreach (var type in typeFilters.Split('|'))
                {
                    int id;
                    if (int.TryParse(type, out id))
                    {
                        Expression<Func<Joke, bool>> comparison = e => e.JokeType_ID == id;

                        expression = expression == null ? comparison : expression.Or(comparison);
                    }
                }
            }

            if (!String.IsNullOrWhiteSpace(textFilters))
            {
                Expression<Func<Joke, bool>> expressionText = null;

                foreach (var text in textFilters.Split('|'))
                {
                    Expression<Func<Joke, bool>> comparison = e => EF.Functions.Like(e.Text, "%" + text + "%");

                    expressionText = expressionText == null ? comparison : expressionText.Or(comparison);

                }

                expression = expression == null ? expressionText : expression.And(expressionText);
            }

            if (expression != null)
                query = query.Where(expression).AsNoTracking();

            var list = Entities.PaginatedList<Joke>.Create(query, pageNumber, pageSize);

            return list;
        }

        public async Task<List<JokeType>> GetJokeTypes(bool onlyActive)
        {
            return await _context.JokeTypes.Where(j => j.IsActive == onlyActive).ToListAsync();
        }

        public async Task<Joke> GetRandomJoke()
        {
            var count = await _context.Jokes.CountAsync();
            var randomNumber = new Random(1);
            return await _context.Jokes.OrderBy(j => Guid.NewGuid()).Skip(randomNumber.Next(count)).Take(1).FirstAsync();
        }
    }
}
