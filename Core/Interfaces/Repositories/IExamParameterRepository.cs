using HealthTrack.Core.Models.Entities;

namespace HealthTrack.Core.Interfaces.Repositories
{
    public interface IExamParameterRepository
    {
        Task<ExamParameter?> GetByIdAsync(int id);
        Task<IEnumerable<ExamParameter>> GetAllAsync();
        Task<IEnumerable<ExamParameter>> GetByExamIdAsync(int examId);
        Task<ExamParameter> AddAsync(ExamParameter examParameter);
        Task<ExamParameter> UpdateAsync(ExamParameter examParameter);
        Task<bool> DeleteAsync(int id);
    }
}