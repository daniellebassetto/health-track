using Microsoft.AspNetCore.Mvc;
using HealthTrack.Areas.Patient.ViewModels;
using HealthTrack.Core.Interfaces.Services;
using HealthTrack.Core.Models.Entities;
using HealthTrack.Controllers;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace HealthTrack.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Authorize]
    public class ExamAnalysisController : BaseController
    {
        private readonly IExamService _examService;
        private readonly IPatientService _patientService;

        public ExamAnalysisController(IExamService examService, IPatientService patientService)
        {
            _examService = examService;
            _patientService = patientService;
        }

        public async Task<IActionResult> Compare()
        {
            try
            {
                var userId = GetCurrentUserId();
                var examListViewModel = await _examService.GetExamListForPatientAsync(userId);
                
                var availableExams = examListViewModel.Exams.Select(e => new ExamSummaryViewModel
                {
                    ExamId = e.ExamId,
                    ExamName = e.ExamName,
                    ExamDate = e.ExamDate,
                    Laboratory = e.Laboratory,
                    ParameterCount = e.ExamParameters?.Count ?? 0
                }).ToList();

                var viewModel = new ExamComparisonViewModel
                {
                    PatientId = 0, // Não precisamos do PatientId aqui
                    AvailableExams = availableExams
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GenerateComparison(ExamComparisonViewModel model)
        {
            try
            {
                var userId = GetCurrentUserId();
                var examListViewModel = await _examService.GetExamListForPatientAsync(userId);
                
                model.AvailableExams = [.. examListViewModel.Exams.Select(e => new ExamSummaryViewModel
                {
                    ExamId = e.ExamId,
                    ExamName = e.ExamName,
                    ExamDate = e.ExamDate,
                    Laboratory = e.Laboratory,
                    ParameterCount = e.ExamParameters?.Count ?? 0
                })];

                if (model.CompareAll && model.AvailableExams.Count < 2)
                {
                    ModelState.AddModelError("", "São necessários pelo menos 2 exames para fazer uma comparação.");
                    return View("Compare", model);
                }
                
                if (!model.CompareAll && model.SelectedExamIds.Count < 2)
                {
                    ModelState.AddModelError("", "Selecione pelo menos 2 exames para comparar.");
                    return View("Compare", model);
                }

                var examIds = model.CompareAll ? model.AvailableExams.Select(e => e.ExamId).ToList() : model.SelectedExamIds;
                var exams = examListViewModel.Exams.Where(e => examIds.Contains(e.ExamId)).OrderBy(e => e.ExamDate).ToList();
                
                var comparedExams = exams.Select(e => new ExamSummaryViewModel
                {
                    ExamId = e.ExamId,
                    ExamName = e.ExamName,
                    ExamDate = e.ExamDate,
                    Laboratory = e.Laboratory,
                    ParameterCount = e.ExamParameters?.Count ?? 0
                }).ToList();

                var allParameters = exams.SelectMany(e => e.ExamParameters ?? new List<ExamParameter>())
                    .Select(p => p.ParameterName).Distinct().OrderBy(p => p).ToList();

                var parameterComparisons = new List<ParameterComparisonViewModel>();
                foreach (var paramName in allParameters)
                {
                    var values = exams.Select(e => new
                    {
                        e.ExamName,
                        e.ExamDate,
                        Parameter = e.ExamParameters?.FirstOrDefault(p => p.ParameterName == paramName)
                    }).Where(x => x.Parameter != null).ToList();

                    if (values.Count != 0)
                    {
                        parameterComparisons.Add(new ParameterComparisonViewModel
                        {
                            ParameterName = paramName,
                            ReferenceRange = values.First().Parameter?.ReferenceRange,
                            Values = [.. values.Select(v => new ParameterValueViewModel
                            {
                                ExamId = v.Parameter?.ExamId ?? 0,
                                ExamDate = v.ExamDate,
                                NumericValue = v.Parameter?.NumericValue ?? v.Parameter?.TextValue,
                                Unit = v.Parameter?.Unit,
                                Comments = v.Parameter?.Comments
                            })]
                        });
                    }
                }

                var aiAnalysis = await _examService.GenerateComparisonAnalysisAsync(examIds, userId);

                model.ComparisonResult = new ExamComparisonResultViewModel
                {
                    ComparedExams = comparedExams,
                    ParameterComparisons = parameterComparisons,
                    AiAnalysis = aiAnalysis
                };

                return View("ComparisonResult", model);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }
    }
}