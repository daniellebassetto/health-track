using HealthTrack.Core.Interfaces.Repositories;
using HealthTrack.Core.Models.Entities;
using HealthTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Repositories
{
    public class ExamRepository(HealthTrackDbContext context) : BaseRepository<Exam>(context), IExamRepository
    {
        public override async Task<Exam?> GetByIdAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.Patient)
                .Include(e => e.ExamParameters)
                .FirstOrDefaultAsync(e => e.ExamId == id);
        }

        public override async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _context.Exams
                .Include(e => e.Patient)
                .Include(e => e.ExamParameters)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Exams
                .Include(e => e.Patient)
                .Include(e => e.ExamParameters)
                .Where(e => e.PatientId == patientId)
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync();
        }
    }

    public class ExamParameterRepository(HealthTrackDbContext context) : BaseRepository<ExamParameter>(context), IExamParameterRepository
    {
        public async Task<IEnumerable<ExamParameter>> GetByExamIdAsync(int examId)
        {
            return await _context.ExamParameters
                .Where(er => er.ExamId == examId)
                .ToListAsync();
        }
    }
}