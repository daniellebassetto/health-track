using HealthTrack.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthTrack.Infrastructure.Configurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.ToTable("pacientes");

            builder.HasKey(p => p.PatientId);
            builder.Property(p => p.PatientId)
                .HasColumnName("id_paciente")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.FirstName)
                .HasColumnName("primeiro_nome")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(p => p.LastName)
                .HasColumnName("sobrenome")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(p => p.Cpf)
                .HasColumnName("cpf")
                .HasColumnType("varchar(20)");

            builder.Property(p => p.Rg)
                .HasColumnName("rg")
                .HasColumnType("varchar(20)");

            builder.Property(p => p.DateOfBirth)
                .HasColumnName("data_nascimento");

            builder.Property(p => p.Gender)
                .HasColumnName("genero")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.Phone)
                .HasColumnName("telefone")
                .HasColumnType("varchar(15)");

            builder.Property(p => p.Email)
                .HasColumnName("email")
                .HasColumnType("varchar(256)");

            builder.Property(p => p.BloodType)
                .HasColumnName("tipo_sanguineo")
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.MedicalHistory)
                .HasColumnName("historico_medico")
                .HasColumnType("varchar(500)");

            builder.Property(p => p.CreatedAt)
                .HasColumnName("data_criacao")
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .HasColumnName("data_atualizacao")
                .IsRequired();

            builder.Property(p => p.UserId)
                .HasColumnName("id_usuario");
        }
    }
}