using HealthTrack.Areas.Patient.ViewModels;
using HealthTrack.Controllers;
using HealthTrack.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text.RegularExpressions;

namespace HealthTrack.Areas.Patient.Controllers;

[Area("Patient")]
[Authorize]
public class ExamController(IExamService examService) : BaseController
{
    private readonly IExamService _examService = examService;

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = GetCurrentUserId();
            var viewModel = await _examService.GetExamListForPatientAsync(userId);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var viewModel = await _examService.GetExamDetailsForPatientAsync(id, userId);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    public IActionResult Create()
    {
        return View(new CreateExamViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateExamViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.HasValidParameters())
            {
                ModelState.AddModelError("", "Pelo menos um parâmetro deve ser adicionado ao exame.");
                return View(model);
            }

            var userId = GetCurrentUserId();
            var examId = await _examService.CreateAsync(model, userId);
            TempData["Success"] = "Exame criado com sucesso!";
            return RedirectToAction("Details", new { id = examId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    public IActionResult AddParameter(int id)
    {
        var viewModel = new ExamParameterViewModel { ExamId = id };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddParameter(ExamParameterViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetCurrentUserId();
            await _examService.AddParameterAsync(model, userId);
            TempData["Success"] = "Parâmetro adicionado com sucesso!";
            return RedirectToAction("Details", new { id = model.ExamId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> ExtractPdfParameters(IFormFile pdfFile, bool useAi = true)
    {
        try
        {
            if (pdfFile == null || pdfFile.Length == 0)
                return Json(new { success = false, message = "Nenhum arquivo foi enviado." });

            if (!pdfFile.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                return Json(new { success = false, message = "Apenas arquivos PDF são aceitos." });

            var parameters = useAi
                ? await _examService.ExtractParametersFromPdfAsync(pdfFile)
                : await _examService.ExtractParametersFromPdfAsync(pdfFile);

            if (parameters == null || !parameters.Any())
                return Json(new { success = false, message = "Não foi possível extrair parâmetros do PDF." });

            return Json(new { success = true, parameters });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Erro ao processar o PDF: " + ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> GenerateSummary(int id)
    {
        try
        {
            var summary = await _examService.GenerateAiSummaryAsync(id, GetCurrentUserId());
            return Json(new { success = true, summary });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Erro ao gerar resumo: " + ex.Message });
        }
    }

    public async Task<IActionResult> GeneratePdf(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var examDetails = await _examService.GetExamDetailsForPatientAsync(id, userId);
            
            using var ms = new MemoryStream();
            var document = new Document(PageSize.A4, 50, 50, 50, 50);
            PdfWriter.GetInstance(document, ms);
            document.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);
            var smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);

            document.Add(new Paragraph(examDetails.Exam.ExamName, titleFont) { Alignment = Element.ALIGN_CENTER, SpacingAfter = 10 });
            document.Add(new Paragraph($"Data: {examDetails.Exam.ExamDate:dd/MM/yyyy}", normalFont) { SpacingAfter = 5 });
            if (!string.IsNullOrWhiteSpace(examDetails.Exam.Laboratory))
                document.Add(new Paragraph($"Laboratório: {examDetails.Exam.Laboratory}", normalFont) { SpacingAfter = 20 });

            if (!string.IsNullOrWhiteSpace(examDetails.Exam.AiSummary))
            {
                document.Add(new Paragraph("Resumo da IA", headerFont) { SpacingAfter = 10 });
                var cleanSummary = Regex.Replace(examDetails.Exam.AiSummary, "<.*?>", string.Empty);
                document.Add(new Paragraph(cleanSummary, normalFont) { SpacingAfter = 20 });
            }

            document.Add(new Paragraph("Parâmetros do Exame", headerFont) { SpacingAfter = 10 });

            var table = new PdfPTable(4) { WidthPercentage = 100, SpacingAfter = 10 };
            table.SetWidths(new float[] { 3, 2, 2, 2 });
            
            var headerCellFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
            table.AddCell(new PdfPCell(new Phrase("Parâmetro", headerCellFont)) { BackgroundColor = new BaseColor(240, 240, 240), Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase("Valor", headerCellFont)) { BackgroundColor = new BaseColor(240, 240, 240), Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase("Unidade", headerCellFont)) { BackgroundColor = new BaseColor(240, 240, 240), Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase("Ref.", headerCellFont)) { BackgroundColor = new BaseColor(240, 240, 240), Padding = 5 });

            foreach (var param in examDetails.Exam.ExamParameters)
            {
                table.AddCell(new PdfPCell(new Phrase(param.ParameterName)) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase(param.NumericValue ?? param.TextValue ?? "-")) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase(param.Unit ?? "-")) { Padding = 5 });
                table.AddCell(new PdfPCell(new Phrase(param.ReferenceRange ?? "-")) { Padding = 5 });
            }

            document.Add(table);
            document.Close();

            return File(ms.ToArray(), "application/pdf", $"Exame_{examDetails.Exam.ExamName.Replace(" ", "_")}_{examDetails.Exam.ExamDate:yyyyMMdd}.pdf");
        }
        catch (Exception ex)
        {
            return BadRequest("Erro ao gerar PDF: " + ex.Message);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            var viewModel = await _examService.GetEditViewModelAsync(id, userId);
            return View(viewModel);
        }
        catch (Exception ex)
        {
            return HandleError(ex);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditExamViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = GetCurrentUserId();
            await _examService.UpdateAsync(model, userId);
            TempData["Success"] = "Exame atualizado com sucesso!";
            return RedirectToAction("Details", new { id = model.ExamId });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _examService.DeleteAsync(id, userId);
            TempData["Success"] = "Exame excluído com sucesso!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Details", new { id });
        }
    }
}