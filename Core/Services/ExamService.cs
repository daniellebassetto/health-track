using HealthTrack.Areas.Patient.ViewModels;
using HealthTrack.Core.Constants;
using HealthTrack.Core.Interfaces.Repositories;
using HealthTrack.Core.Interfaces.Services;
using HealthTrack.Core.Models.Entities;
using HealthTrack.Core.Utils;

namespace HealthTrack.Core.Services;

public class ExamService(IExamRepository examRepository, IExamParameterRepository examParameterRepository, IPatientRepository patientRepository, AiExamExtractor aiExtractor) : IExamService
{
    private readonly IExamRepository _examRepository = examRepository;
    private readonly IExamParameterRepository _examParameterRepository = examParameterRepository;
    private readonly IPatientRepository _patientRepository = patientRepository;
    private readonly AiExamExtractor _aiExtractor = aiExtractor;

    public async Task<Exam?> GetByIdForPatientAsync(int examId, int patientId)
    {
        var exam = await _examRepository.GetByIdAsync(examId).ConfigureAwait(false);
        return exam?.PatientId == patientId ? exam : null;
    }

    public async Task<int> CreateAsync(CreateExamViewModel model, string userId)
    {
        ArgumentNullException.ThrowIfNull(model);

        var patient = await GetPatientByUserIdAsync(userId).ConfigureAwait(false);
        var exam = new Exam
        {
            ExamName = model.ExamName,
            ExamDate = model.ExamDate,
            Notes = model.Notes,
            PatientId = patient.PatientId,
            Laboratory = model.Laboratory,
            HasInsurance = model.HasInsurance,
            InsuranceName = model.InsuranceName,
            DoctorName = model.DoctorName,
            DoctorCrm = model.DoctorCrm,
            FastingHours = model.FastingHours
        };

        var createdExam = await CreateEntityAsync(exam).ConfigureAwait(false);

        // Adicionar parâmetros se existirem
        var validParameters = model.ExamParameters
            .Where(p => !string.IsNullOrWhiteSpace(p.ParameterName))
            .ToList();

        foreach (var parameterModel in validParameters)
        {
            var parameter = new ExamParameter
            {
                ExamId = createdExam.ExamId,
                ParameterName = parameterModel.ParameterName,
                NumericValue = parameterModel.NumericValue,
                Unit = parameterModel.Unit,
                ReferenceRange = parameterModel.ReferenceRange,
                Comments = parameterModel.Comments
            };

            await AddParameterEntityAsync(parameter).ConfigureAwait(false);
        }

        return createdExam.ExamId;
    }

    public async Task AddParameterAsync(ExamParameterViewModel model, string userId)
    {
        ArgumentNullException.ThrowIfNull(model);

        var patient = await GetPatientByUserIdAsync(userId).ConfigureAwait(false);
        await ValidateExamOwnershipAsync(model.ExamId, patient.PatientId).ConfigureAwait(false);

        var parameter = new ExamParameter
        {
            ExamId = model.ExamId,
            ParameterName = model.ParameterName,
            NumericValue = model.NumericValue,
            Unit = model.Unit,
            ReferenceRange = model.ReferenceRange
        };

        await AddParameterEntityAsync(parameter).ConfigureAwait(false);
    }

    public async Task<ExamListViewModel> GetExamListForPatientAsync(string userId)
    {
        var patient = await GetPatientByUserIdAsync(userId).ConfigureAwait(false);
        var exams = await _examRepository.GetByPatientIdAsync(patient.PatientId).ConfigureAwait(false);

        return new ExamListViewModel
        {
            Exams = exams,
            PatientName = patient.GetFullName()
        };
    }

    public async Task<ExamDetailsViewModel> GetExamDetailsForPatientAsync(int examId, string userId)
    {
        var patient = await GetPatientByUserIdAsync(userId).ConfigureAwait(false);
        var exam = await GetByIdForPatientAsync(examId, patient.PatientId).ConfigureAwait(false)
            ?? throw new ArgumentException(ErrorMessages.ExamNotFound);

        return new ExamDetailsViewModel
        {
            Exam = exam,
            PatientName = patient.GetFullName()
        };
    }

    public async Task<string> GenerateAiSummaryAsync(int examId, string userId)
    {
        try
        {
            var examDetails = await GetExamDetailsForPatientAsync(examId, userId);
            var summary = await _aiExtractor.GenerateExamSummaryAsync(examDetails.Exam);
            
            examDetails.Exam.AiSummary = summary;
            examDetails.Exam.SetForUpdate();
            await _examRepository.UpdateAsync(examDetails.Exam);
            
            return summary;
        }
        catch
        {
            return "A funcionalidade de resumo não está funcionando no momento. Tente novamente mais tarde.";
        }
    }

    public async Task<List<PdfParameterDto>> ExtractParametersFromPdfAsync(IFormFile pdfFile)
    {
        try
        {
            return await _aiExtractor.ExtractParametersFromPdfAsync(pdfFile);
        }
        catch
        {
            return [];
        }
    }

    public async Task<EditExamViewModel> GetEditViewModelAsync(int examId, string userId)
    {
        var patient = await GetPatientByUserIdAsync(userId).ConfigureAwait(false);
        var exam = await GetByIdForPatientAsync(examId, patient.PatientId).ConfigureAwait(false)
            ?? throw new ArgumentException(ErrorMessages.ExamNotFound);

        return new EditExamViewModel
        {
            ExamId = exam.ExamId,
            PatientId = exam.PatientId,
            ExamName = exam.ExamName,
            ExamDate = exam.ExamDate,
            Notes = exam.Notes,
            Laboratory = exam.Laboratory,
            HasInsurance = exam.HasInsurance,
            InsuranceName = exam.InsuranceName,
            DoctorName = exam.DoctorName,
            DoctorCrm = exam.DoctorCrm,
            FastingHours = exam.FastingHours,
            ExamParameters = exam.ExamParameters.Select(p => new ExamParameterViewModel
            {
                ExamParameterId = p.ExamParameterId,
                ExamId = p.ExamId,
                ParameterName = p.ParameterName,
                NumericValue = p.NumericValue,
                Unit = p.Unit,
                ReferenceRange = p.ReferenceRange,
                Comments = p.Comments
            }).ToList()
        };
    }

    public async Task UpdateAsync(EditExamViewModel model, string userId)
    {
        ArgumentNullException.ThrowIfNull(model);

        var patient = await GetPatientByUserIdAsync(userId).ConfigureAwait(false);
        var exam = await GetByIdForPatientAsync(model.ExamId, patient.PatientId).ConfigureAwait(false)
            ?? throw new ArgumentException(ErrorMessages.ExamNotFound);

        exam.ExamName = model.ExamName;
        exam.ExamDate = model.ExamDate.Date;
        exam.Notes = model.Notes;
        exam.Laboratory = model.Laboratory;
        exam.HasInsurance = model.HasInsurance;
        exam.InsuranceName = model.InsuranceName;
        exam.DoctorName = model.DoctorName;
        exam.DoctorCrm = model.DoctorCrm;
        exam.FastingHours = model.FastingHours;
        exam.SetForUpdate();

        await _examRepository.UpdateAsync(exam).ConfigureAwait(false);

        var existingParameters = exam.ExamParameters.ToList();
        foreach (var existingParam in existingParameters)
        {
            await _examParameterRepository.DeleteAsync(existingParam.ExamParameterId).ConfigureAwait(false);
        }

        var validParameters = model.ExamParameters
            .Where(p => !string.IsNullOrWhiteSpace(p.ParameterName))
            .ToList();

        foreach (var parameterModel in validParameters)
        {
            var parameter = new ExamParameter
            {
                ExamId = exam.ExamId,
                ParameterName = parameterModel.ParameterName,
                NumericValue = parameterModel.NumericValue,
                Unit = parameterModel.Unit,
                ReferenceRange = parameterModel.ReferenceRange,
                Comments = parameterModel.Comments
            };

            await AddParameterEntityAsync(parameter).ConfigureAwait(false);
        }
    }

    public async Task DeleteAsync(int examId, string userId)
    {
        var patient = await GetPatientByUserIdAsync(userId).ConfigureAwait(false);
        var exam = await GetByIdForPatientAsync(examId, patient.PatientId).ConfigureAwait(false)
            ?? throw new ArgumentException(ErrorMessages.ExamNotFound);

        await _examRepository.DeleteAsync(exam.ExamId).ConfigureAwait(false);
    }

    public async Task<string> GenerateComparisonAnalysisAsync(List<int> examIds, string userId)
    {
        try
        {
            var patient = await GetPatientByUserIdAsync(userId).ConfigureAwait(false);
            var exams = new List<Exam>();
            
            foreach (var examId in examIds)
            {
                var exam = await GetByIdForPatientAsync(examId, patient.PatientId).ConfigureAwait(false);
                if (exam != null) exams.Add(exam);
            }

            if (exams.Count < 2)
                throw new ArgumentException("São necessários pelo menos 2 exames para comparação.");

            return await _aiExtractor.GenerateComparisonAnalysisAsync(exams);
        }
        catch
        {
            return "A funcionalidade de comparação não está funcionando no momento. Tente novamente mais tarde.";
        }
    }

    #region Private Methods
    private async Task<Patient> GetPatientByUserIdAsync(string userId)
    {
        return await _patientRepository.GetByUserIdAsync(userId).ConfigureAwait(false)
            ?? throw new ArgumentException(ErrorMessages.PatientNotFound);
    }

    private async Task ValidateExamOwnershipAsync(int examId, int patientId)
    {
        _ = await GetByIdForPatientAsync(examId, patientId).ConfigureAwait(false)
            ?? throw new ArgumentException(ErrorMessages.ExamNotFound);
    }

    private async Task<Exam> CreateEntityAsync(Exam exam)
    {
        ArgumentNullException.ThrowIfNull(exam);

        exam.SetForCreation();
        return await _examRepository.AddAsync(exam).ConfigureAwait(false);
    }

    private async Task<ExamParameter> AddParameterEntityAsync(ExamParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        parameter.SetForCreation();
        return await _examParameterRepository.AddAsync(parameter).ConfigureAwait(false);
    }
    #endregion
}