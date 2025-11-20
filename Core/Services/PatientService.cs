using HealthTrack.Areas.Patient.ViewModels;
using HealthTrack.Core.Constants;
using HealthTrack.Core.Interfaces.Repositories;
using HealthTrack.Core.Interfaces.Services;
using HealthTrack.Core.Models.Entities;
using Microsoft.AspNetCore.Identity;


namespace HealthTrack.Core.Services
{
    public class PatientService(IPatientRepository patientRepository, IExamRepository examRepository, UserManager<User> userManager) : IPatientService
    {
        private readonly IPatientRepository _patientRepository = patientRepository;
        private readonly IExamRepository _examRepository = examRepository;
        private readonly UserManager<User> _userManager = userManager;

        public async Task CreateAsync(CreatePatientViewModel model, string userId)
        {
            ArgumentNullException.ThrowIfNull(model);

            var existingPatient = await GetPatientByUserIdAsync(userId);
            if (existingPatient != null)
                throw new InvalidOperationException("Paciente já possui perfil cadastrado");

            await ValidateUserData(model.FirstName, model.LastName, model.Email, userId);
            await ValidateUniqueFields(model.Email, model.Cpf, model.Phone);

            var patient = new Patient
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Cpf = model.Cpf,
                Rg = model.Rg,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Phone = model.Phone,
                Email = model.Email,
                BloodType = model.BloodType,
                MedicalHistory = model.MedicalHistory,
                UserId = userId
            };

            await CreateEntityAsync(patient);
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(string userId)
        {
            var patient = await GetPatientByUserIdAsync(userId) ?? throw new ArgumentException(ErrorMessages.PatientNotFound);

            var exams = await _examRepository.GetByPatientIdAsync(patient.PatientId);
            var examsList = exams.ToList();

            return new DashboardViewModel
            {
                PatientId = patient.PatientId,
                PatientName = patient.GetFullName(),
                TotalExams = examsList.Count,
                RecentExams = examsList.OrderByDescending(e => e.ExamDate).Take(5),
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                DateOfBirth = patient.DateOfBirth,
                Cpf = patient.Cpf,
                Rg = patient.Rg,
                Gender = patient.Gender,
                Phone = patient.Phone,
                BloodType = patient.BloodType,
                MedicalHistory = patient.MedicalHistory,
                CreatedAt = patient.CreatedAt,
                UpdatedAt = patient.UpdatedAt
            };
        }

        public async Task<EditPatientViewModel> GetForEditAsync(string userId)
        {
            var patient = await GetPatientByUserIdAsync(userId) ?? throw new ArgumentException(ErrorMessages.PatientNotFound);

            return new EditPatientViewModel
            {
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Cpf = patient.Cpf,
                Rg = patient.Rg,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Phone = patient.Phone,
                Email = patient.Email,
                BloodType = patient.BloodType,
                MedicalHistory = patient.MedicalHistory
            };
        }

        public async Task UpdateAsync(EditPatientViewModel model, string userId)
        {
            ArgumentNullException.ThrowIfNull(model);

            var existingPatient = await GetPatientByUserIdAsync(userId) ?? throw new ArgumentException(ErrorMessages.PatientNotFound);

            await ValidateUserData(model.FirstName, model.LastName, model.Email, userId);
            await ValidateUniqueFields(model.Email, model.Cpf, model.Phone, existingPatient.PatientId);

            existingPatient.Update(model);
            await UpdateEntityAsync(existingPatient);
        }

        #region Private Methods
        private async Task<Patient?> GetPatientByUserIdAsync(string userId)
        {
            return await _patientRepository.GetByUserIdAsync(userId);
        }

        private async Task ValidateUserData(string firstName, string lastName, string? email, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user?.FirstName != firstName)
                throw new InvalidOperationException("O nome do paciente deve ser o mesmo do usuário logado");

            if (user?.LastName != lastName)
                throw new InvalidOperationException("O sobrenome do paciente deve ser o mesmo do usuário logado");

            if (!string.IsNullOrWhiteSpace(email) && user?.Email != email)
                throw new InvalidOperationException("O e-mail do paciente deve ser o mesmo do usuário logado");
        }

        private async Task ValidateUniqueFields(string? email, string? cpf, string? phone, int? excludePatientId = null)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailExists = await _patientRepository.ExistsByEmailAsync(email, excludePatientId);
                if (emailExists)
                    throw new InvalidOperationException("Já existe um paciente cadastrado com este e-mail");
            }

            if (!string.IsNullOrWhiteSpace(cpf))
            {
                var cpfExists = await _patientRepository.ExistsByCpfAsync(cpf, excludePatientId);
                if (cpfExists)
                    throw new InvalidOperationException("Já existe um paciente cadastrado com este CPF");
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                var phoneExists = await _patientRepository.ExistsByPhoneAsync(phone, excludePatientId);
                if (phoneExists)
                    throw new InvalidOperationException("Já existe um paciente cadastrado com este telefone");
            }
        }

        private async Task<Patient> CreateEntityAsync(Patient patient)
        {
            ArgumentNullException.ThrowIfNull(patient);
            patient.SetForCreation();
            return await _patientRepository.AddAsync(patient);
        }

        private async Task<Patient> UpdateEntityAsync(Patient patient)
        {
            ArgumentNullException.ThrowIfNull(patient);
            patient.SetForUpdate();
            return await _patientRepository.UpdateAsync(patient);
        }
        #endregion
    }
}