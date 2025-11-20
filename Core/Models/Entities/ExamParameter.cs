
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthTrack.Core.Models.Entities
{
    public class ExamParameter
    {
        [Key]
        public int ExamParameterId { get; set; }

        [Required]
        public int ExamId { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam? Exam { get; set; }

        [Required]
        [StringLength(100)]
        public string ParameterName { get; set; } = string.Empty;

        public string? NumericValue { get; set; }

        [StringLength(500)]
        public string? TextValue { get; set; }

        [StringLength(20)]
        public string? Unit { get; set; }

        [StringLength(50)]
        public string? ReferenceRange { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Domain methods
        public void SetForCreation()
        {
            var now = DateTime.UtcNow;
            CreatedAt = now;
            UpdatedAt = now;
        }

        public void SetForUpdate()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}