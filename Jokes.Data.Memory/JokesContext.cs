using Jokes.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Jokes.Data.Memory
{
    public class JokesContext : DbContext
    {
        public JokesContext(DbContextOptions<JokesContext> options) : base(options) { }

        public DbSet<Joke> Jokes { get; set; }
        public DbSet<JokeType> JokeTypes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //do model stuff here
        }
    }
}
