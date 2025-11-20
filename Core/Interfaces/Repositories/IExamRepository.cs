using HealthTrack.Core.Models.Entities;

namespace HealthTrack.Core.Interfaces.Repositories
{
    public interface IExamRepository
    {
        Task<Exam?> GetByIdAsync(int id);
        Task<IEnumerable<Exam>> GetAllAsync();
        Task<IEnumerable<Exam>> GetByPatientIdAsync(int patientId);
        Task<Exam> AddAsync(Exam exam);
        Task<Exam> UpdateAsync(Exam exam);
        Task<bool> DeleteAsync(int id);
    }
}