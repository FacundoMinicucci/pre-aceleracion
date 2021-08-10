using ChallengeDisney.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChallengeDisney.Context
{
    public class ChallengeDisneyContext : DbContext
    {
        public ChallengeDisneyContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);            

        }

        public DbSet<Character> Characters { get; set; } = null!;
        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;       
 
    }
}
