using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Report.Domain.Models;

namespace Report.Infra.Contexts.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        { 
            builder.HasKey(user => user.Id);

            builder.Property(user => user.Id)
                   .IsRequired();

            builder.Property(user => user.Name)
                   .HasColumnType("varchar(60)")
                   .IsRequired();

            builder.Property(user => user.Email)
                   .HasColumnType("varchar(60)")
                   .IsRequired();

            builder.Property(user => user.Hash)
                   .HasColumnType("varchar(60)")
                   .IsRequired();

            builder.Property(user => user.Salt)
                   .HasColumnType("varchar(60)")
                   .IsRequired();

            builder.Property(user => user.Role)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(user => user.CreatedAt)
                   .IsRequired();
        }
    }
}