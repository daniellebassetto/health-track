using HealthTrack.Core.Models.Entities;

namespace HealthTrack.Areas.Patient.ViewModels
{
    public class ExamListViewModel
    {
        public IEnumerable<Exam> Exams { get; set; } = new List<Exam>();
        public string PatientName { get; set; } = string.Empty;
    }
}