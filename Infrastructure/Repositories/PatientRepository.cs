using HealthTrack.Core.Interfaces.Repositories;
using HealthTrack.Core.Models.Entities;
using HealthTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Repositories
{
    public class PatientRepository(HealthTrackDbContext context) : BaseRepository<Patient>(context), IPatientRepository
    {
        public async Task<Patient?> GetByUserIdAsync(string userId)
        {
            return await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludePatientId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var query = _context.Patients.Where(p => p.Email == email);
            if (excludePatientId.HasValue)
                query = query.Where(p => p.PatientId != excludePatientId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByCpfAsync(string cpf, int? excludePatientId = null)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            var query = _context.Patients.Where(p => p.Cpf == cpf);
            if (excludePatientId.HasValue)
                query = query.Where(p => p.PatientId != excludePatientId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByPhoneAsync(string phone, int? excludePatientId = null)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            var query = _context.Patients.Where(p => p.Phone == phone);
            if (excludePatientId.HasValue)
                query = query.Where(p => p.PatientId != excludePatientId.Value);

            return await query.AnyAsync();
        }
    }
}