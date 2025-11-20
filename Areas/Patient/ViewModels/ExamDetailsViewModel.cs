using HealthTrack.Core.Models.Entities;

namespace HealthTrack.Areas.Patient.ViewModels
{
    public class ExamDetailsViewModel
    {
        public Exam Exam { get; set; } = new();
        public string PatientName { get; set; } = string.Empty;
    }
}