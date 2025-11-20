using HealthTrack.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HealthTrack.Infrastructure.Configurations
{
    public class ExamParameterConfiguration : IEntityTypeConfiguration<ExamParameter>
    {
        public void Configure(EntityTypeBuilder<ExamParameter> builder)
        {
            builder.ToTable("parametros_exame");

            builder.HasKey(ep => ep.ExamParameterId);
            builder.Property(ep => ep.ExamParameterId).HasColumnName("id_parametro_exame").ValueGeneratedOnAdd();

            builder.Property(ep => ep.ExamId)
                .HasColumnName("id_exame")
                .IsRequired();

            builder.Property(ep => ep.ParameterName)
                .HasColumnName("nome_parametro")
                .HasColumnType("varchar(100)")
                .IsRequired();

            builder.Property(ep => ep.NumericValue)
                .HasColumnName("valor_numerico")
                .HasColumnType("varchar(50)");

            builder.Property(ep => ep.TextValue)
                .HasColumnName("valor_texto")
                .HasColumnType("varchar(500)");

            builder.Property(ep => ep.Unit)
                .HasColumnName("unidade")
                .HasColumnType("varchar(20)");

            builder.Property(ep => ep.ReferenceRange)
                .HasColumnName("faixa_referencia")
                .HasColumnType("varchar(50)");

            builder.Property(ep => ep.Comments)
                .HasColumnName("comentarios")
                .HasColumnType("varchar(500)");

            builder.Property(ep => ep.CreatedAt)
                .HasColumnName("data_criacao")
                .IsRequired();

            builder.Property(ep => ep.UpdatedAt)
                .HasColumnName("data_atualizacao")
                .IsRequired();

            builder.HasOne(ep => ep.Exam)
                   .WithMany(e => e.ExamParameters)
                   .HasForeignKey(ep => ep.ExamId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}