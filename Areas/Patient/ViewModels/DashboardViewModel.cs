using HealthTrack.Core.Models.Entities;
using HealthTrack.Core.Models.Enums;

namespace HealthTrack.Areas.Patient.ViewModels
{
    public class DashboardViewModel
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int TotalExams { get; set; }
        public IEnumerable<Exam> RecentExams { get; set; } = new List<Exam>();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Cpf { get; set; }
        public string? Rg { get; set; }
        public Gender Gender { get; set; }
        public string? Phone { get; set; }
        public BloodType BloodType { get; set; }
        public string? MedicalHistory { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}