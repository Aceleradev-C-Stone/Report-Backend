using Microsoft.EntityFrameworkCore;
using Report.Infra.Contexts.Configurations;
using Report.Core.Models;

namespace Report.Infra.Contexts
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                        .HasMany<Log>(user => user.Logs)
                        .WithOne(log => log.User)
                        .HasForeignKey(log => log.UserId);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new LogConfiguration());
        }
    }
}