using HealthTrack.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthTrack.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("usuarios");

            builder.Property(u => u.FirstName)
                .HasColumnName("primeiro_nome")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(u => u.LastName)
                .HasColumnName("sobrenome")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(u => u.Role)
                .HasColumnName("role")
                .HasConversion<int>();

            builder.Property(u => u.CreatedAt)
                .HasColumnName("data_criacao")
                .IsRequired();

            builder.HasOne(u => u.Patient)
                   .WithOne(p => p.User)
                   .HasForeignKey<Patient>(p => p.UserId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}