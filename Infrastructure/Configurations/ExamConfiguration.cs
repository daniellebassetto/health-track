using HealthTrack.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthTrack.Infrastructure.Configurations
{
    public class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.ToTable("exames");

            builder.HasKey(e => e.ExamId);
            builder.Property(e => e.ExamId).HasColumnName("id_exame").ValueGeneratedOnAdd();

            builder.Property(e => e.PatientId)
                .HasColumnName("id_paciente")
                .IsRequired();

            builder.Property(e => e.ExamName)
                .HasColumnName("nome_exame")
                .HasColumnType("varchar(200)")
                .IsRequired();

            builder.Property(e => e.ExamDate)
                .HasColumnName("data_exame")
                .IsRequired();

            builder.Property(e => e.Notes)
                .HasColumnName("observacoes")
                .HasColumnType("varchar(500)");

            builder.Property(e => e.CreatedAt)
                .HasColumnName("data_criacao")
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .HasColumnName("data_atualizacao")
                .IsRequired();

            builder.Property(e => e.AiSummary)
                .HasColumnName("resumo_ia")
                .HasColumnType("TEXT");

            builder.Property(e => e.Laboratory)
                .HasColumnName("laboratorio")
                .HasColumnType("varchar(200)");

            builder.Property(e => e.HasInsurance)
                .HasColumnName("tem_convenio")
                .IsRequired();

            builder.Property(e => e.InsuranceName)
                .HasColumnName("nome_convenio")
                .HasColumnType("varchar(100)");

            builder.Property(e => e.DoctorName)
                .HasColumnName("nome_medico_solicitante")
                .HasColumnType("varchar(100)");

            builder.Property(e => e.DoctorCrm)
                .HasColumnName("crm_medico_solicitante")
                .HasColumnType("varchar(50)");

            builder.HasOne(e => e.Patient)
                   .WithMany(p => p.Exams)
                   .HasForeignKey(e => e.PatientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}