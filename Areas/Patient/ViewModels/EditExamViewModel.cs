using System.ComponentModel.DataAnnotations;

namespace HealthTrack.Areas.Patient.ViewModels
{
    public class EditExamViewModel
    {
        public int ExamId { get; set; }
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Exam name is required")]
        [StringLength(200, ErrorMessage = "Exam name cannot exceed 200 characters")]
        public string ExamName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Exam date is required")]
        public DateTime ExamDate { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        [StringLength(200, ErrorMessage = "Laboratory name cannot exceed 200 characters")]
        public string? Laboratory { get; set; }

        public bool HasInsurance { get; set; }

        [StringLength(100, ErrorMessage = "Insurance name cannot exceed 100 characters")]
        public string? InsuranceName { get; set; }

        [Display(Name = "Médico que Solicitou o Exame")]
        [StringLength(100, ErrorMessage = "Doctor name cannot exceed 100 characters")]
        public string? DoctorName { get; set; }

        [Display(Name = "CRM do Médico Solicitante")]
        [StringLength(50, ErrorMessage = "CRM cannot exceed 50 characters")]
        public string? DoctorCrm { get; set; }
        
        [Display(Name = "Horas de Jejum")]
        [Range(0, 72, ErrorMessage = "Horas de jejum deve estar entre 0 e 72 horas")]
        public int? FastingHours { get; set; }

        public List<ExamParameterViewModel> ExamParameters { get; set; } = new List<ExamParameterViewModel>();
    }
}