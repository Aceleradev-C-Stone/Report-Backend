using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Domain.Models;

namespace Report.Data.Context.Configurations
{
    public class LogConfiguration : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            builder.HasKey(log => log.Id);

            builder.Property(log => log.Id)
                   .IsRequired();

            builder.Property(log => log.Description)
                   .HasColumnType("varchar(255)")
                   .IsRequired();

            builder.Property(log => log.Title)
                   .HasColumnType("varchar(60)")
                   .IsRequired();

            builder.Property(log => log.Details)
                   .HasColumnType("varchar(max)")
                   .IsRequired();

            builder.Property(log => log.Source)
                   .HasColumnType("varchar(60)")
                   .IsRequired();

            builder.Property(log => log.EventCount)
                   .HasDefaultValue(0)
                   .IsRequired();

            builder.Property(log => log.Level)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(log => log.Channel)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(log => log.CreatedAt)
                   .IsRequired();
        }
    }
}