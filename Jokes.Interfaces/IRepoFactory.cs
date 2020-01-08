using Jokes.Data.Memory;

namespace Jokes.Interfaces
{
    public interface IRepoFactory
    {
        JokesContext Context { set;  }
        IJokesRepo JokesRepo { get; }
    }
}
