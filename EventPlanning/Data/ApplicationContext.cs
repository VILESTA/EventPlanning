using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EventPlanning.Data
{
    public class ApplicationContext: DbContext
    {
        public DbSet<EventPlanning.User> Users { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
