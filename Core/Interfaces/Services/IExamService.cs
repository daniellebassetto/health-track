using HealthTrack.Areas.Patient.ViewModels;

namespace HealthTrack.Core.Interfaces.Services
{
    public interface IExamService
    {
        Task<int> CreateAsync(CreateExamViewModel model, string userId);
        Task AddParameterAsync(ExamParameterViewModel model, string userId);
        Task<ExamListViewModel> GetExamListForPatientAsync(string userId);
        Task<ExamDetailsViewModel> GetExamDetailsForPatientAsync(int examId, string userId);
        Task<List<PdfParameterDto>> ExtractParametersFromPdfAsync(IFormFile pdfFile);
        Task<string> GenerateAiSummaryAsync(int examId, string userId);
        Task<EditExamViewModel> GetEditViewModelAsync(int examId, string userId);
        Task UpdateAsync(EditExamViewModel model, string userId);
        Task DeleteAsync(int examId, string userId);
        Task<string> GenerateComparisonAnalysisAsync(List<int> examIds, string userId);
    }

    public class PdfParameterDto
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }
}