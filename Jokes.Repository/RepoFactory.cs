using Jokes.Data.Memory;
using Jokes.Interfaces;

namespace Jokes.Repository
{
    public class RepoFactory : IRepoFactory
    {
        internal JokesContext _context;

        public RepoFactory() { }

        public JokesContext Context { set { _context = value; } }

        private JokesRepo _jokesRepo;
        public IJokesRepo JokesRepo
        {
            get
            {
                if (_jokesRepo == null)
                    this._jokesRepo = new JokesRepo(this);
                return this._jokesRepo;
            }
        }
    }
}
