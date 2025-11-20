using System.ComponentModel.DataAnnotations;

namespace HealthTrack.Areas.Patient.ViewModels
{
    public class ExamComparisonViewModel
    {
        public int PatientId { get; set; }
        public List<ExamSummaryViewModel> AvailableExams { get; set; } = new();
        public List<int> SelectedExamIds { get; set; } = new();
        public bool CompareAll { get; set; }
        public ExamComparisonResultViewModel? ComparisonResult { get; set; }
    }

    public class ExamSummaryViewModel
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public string? Laboratory { get; set; }
        public int ParameterCount { get; set; }
        public bool IsSelected { get; set; }
    }

    public class ExamComparisonResultViewModel
    {
        public List<ExamSummaryViewModel> ComparedExams { get; set; } = new();
        public List<ParameterComparisonViewModel> ParameterComparisons { get; set; } = new();
        public string? AiAnalysis { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    public class ParameterComparisonViewModel
    {
        public string ParameterName { get; set; } = string.Empty;
        public List<ParameterValueViewModel> Values { get; set; } = new();
        public string? Trend { get; set; } // "Increasing", "Decreasing", "Stable", "Irregular"
        public bool IsWithinNormalRange { get; set; }
        public string? ReferenceRange { get; set; }
    }

    public class ParameterValueViewModel
    {
        public int ExamId { get; set; }
        public DateTime ExamDate { get; set; }
        public string? NumericValue { get; set; }
        public string? Unit { get; set; }
        public string? Comments { get; set; }
        public bool IsNormal { get; set; }
    }
}