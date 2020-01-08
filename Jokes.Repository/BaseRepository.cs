using Jokes.Data.Memory;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jokes.Repository
{
    public class BaseRepository
    {
        protected RepoFactory _RepoFactory;
        protected RepoFactory RepoFactory
        {
            get { return this._RepoFactory; }
        }

        protected JokesContext _context;

        public BaseRepository(RepoFactory repoFactory)
        {
            this._RepoFactory = repoFactory;
            _context = repoFactory._context;
        }

        protected virtual async Task<T> GetOne<T, A>(A id) where T : class
        {
            T entity = await _context.Set<T>().FindAsync(id);
            return entity;
        }

        protected virtual IEnumerable<T> Get<T>(string queryString = null) where T : class
        {
            return _context.Set<T>();
        }

        protected virtual async Task<T> Add<T>(T entity) where T : class
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        protected virtual async Task<bool> Save<T, A>(T entity, A id) where T : class
        {
            //before we update we need to get the existing value
            var existingValue = await _context.Set<T>().FindAsync(id);

            //if we found nothing then throw error
            if (existingValue == null)
            {
                return false;
            }

            //detach it so we can use it without affect
            _context.Entry(existingValue).State = EntityState.Detached;

            //attach our updated entity
            _context.Entry<T>(entity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var item = _context.Set<T>().Find(id);
                //if (!Exists<T>(id))
                if (item == null)
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

            return true;
        }

        protected virtual async Task<bool> Delete<T>(Guid ids, bool cascade = false) where T : class
        {
            T entity = await _context.Set<T>().FindAsync(ids);
            if (entity == null)
            {
                return false;
            }

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        protected virtual async Task<bool> Delete<T>(T entity) where T : class
        {
            //attach the entity
            _context.Set<T>().Attach(entity);
            //delete the object
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
