using System.ComponentModel.DataAnnotations;

namespace HealthTrack.Areas.Patient.ViewModels
{
    public class ExamParameterViewModel
    {
        public int ExamParameterId { get; set; }

        public int ExamId { get; set; }

        [Required(ErrorMessage = "Parameter name is required")]
        [StringLength(100, ErrorMessage = "Parameter name cannot exceed 100 characters")]
        public string ParameterName { get; set; } = string.Empty;

        public string? NumericValue { get; set; }

        [StringLength(500, ErrorMessage = "Text value cannot exceed 500 characters")]
        public string? TextValue { get; set; }

        [StringLength(20, ErrorMessage = "Unit cannot exceed 20 characters")]
        public string? Unit { get; set; }

        [StringLength(50, ErrorMessage = "Reference range cannot exceed 50 characters")]
        public string? ReferenceRange { get; set; }

        [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
        public string? Comments { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}