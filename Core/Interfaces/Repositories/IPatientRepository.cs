using HealthTrack.Core.Models.Entities;

namespace HealthTrack.Core.Interfaces.Repositories
{
    public interface IPatientRepository
    {
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient?> GetByUserIdAsync(string userId);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient> AddAsync(Patient patient);
        Task<Patient> UpdateAsync(Patient patient);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsByEmailAsync(string email, int? excludePatientId = null);
        Task<bool> ExistsByCpfAsync(string cpf, int? excludePatientId = null);
        Task<bool> ExistsByPhoneAsync(string phone, int? excludePatientId = null);
    }
}