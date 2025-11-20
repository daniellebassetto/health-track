using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTrack.Core.Models.Entities
{
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        [Required]
        [StringLength(200)]
        public string ExamName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime ExamDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public string? AiSummary { get; set; }
        
        [StringLength(200)]
        public string? Laboratory { get; set; }
        
        public bool HasInsurance { get; set; }
        
        [StringLength(100)]
        public string? InsuranceName { get; set; }
        
        [Display(Name = "Médico que Solicitou o Exame")]
        [StringLength(100)]
        public string? DoctorName { get; set; }
        
        [Display(Name = "CRM do Médico Solicitante")]
        [StringLength(50)]
        public string? DoctorCrm { get; set; }
        
        [Display(Name = "Horas de Jejum")]
        public int? FastingHours { get; set; }

        public virtual ICollection<ExamParameter> ExamParameters { get; set; } = [];

        public void SetForCreation()
        {
            var now = DateTime.UtcNow;
            ExamDate = ExamDate.Date;
            CreatedAt = now;
            UpdatedAt = now;
        }

        public void SetForUpdate()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}