using Microsoft.EntityFrameworkCore;
using Report.Data.Context.Configurations;
using Report.Domain.Models;

namespace Report.Data.Context
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