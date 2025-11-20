using HealthTrack.Core.Interfaces.Services;
using HealthTrack.Core.Models;
using HealthTrack.Core.Models.Entities;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;

namespace HealthTrack.Core.Utils;

public class AiExamExtractor : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly GeminiApiOptions _options;
    private readonly ILogger<AiExamExtractor> _logger;

    public AiExamExtractor(IOptions<GeminiApiOptions> options, ILogger<AiExamExtractor> logger)
    {
        _options = options.Value;
        _logger = logger;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "HealthTrack/1.0");
    }

    public async Task<List<PdfParameterDto>> ExtractParametersFromPdfAsync(IFormFile pdfFile)
    {
        try
        {
            _logger.LogInformation("Iniciando extração de parâmetros do PDF: {FileName}", pdfFile.FileName);

            var pdfText = ExtractTextFromPdf(pdfFile);
            if (string.IsNullOrWhiteSpace(pdfText))
            {
                _logger.LogWarning("Nenhum texto extraído do PDF");
                return [];
            }

            return await ExtractParametersWithAi(pdfText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao extrair parâmetros do PDF: {FileName}", pdfFile.FileName);
            return [];
        }
    }

    private static string ExtractTextFromPdf(IFormFile pdfFile)
    {
        using var stream = pdfFile.OpenReadStream();
        using var document = PdfDocument.Open(stream);

        var text = new StringBuilder();
        foreach (var page in document.GetPages())
        {
            text.AppendLine(page.Text);
        }

        return text.ToString();
    }

    private async Task<List<PdfParameterDto>> ExtractParametersWithAi(string pdfText)
    {
        try
        {
            var prompt = $"Extraia TODOS os parâmetros de exames médicos do texto incluindo nome, valor, unidade e referência. " +
                $"Retorne JSON: [{{\"name\":\"HEMOGLOBINA\",\"value\":\"11.6\",\"unit\":\"g/dL\",\"reference\":\"12.0 - 16.0\"}}]\n\nTexto:\n{pdfText}";
            
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    maxOutputTokens = _options.MaxTokens,
                    temperature = _options.Temperature
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Remove("X-goog-api-key");
            _httpClient.DefaultRequestHeaders.Add("X-goog-api-key", _options.ApiKey);
            
            var response = await _httpClient.PostAsync(_options.BaseUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro na API Gemini: {StatusCode} - {Content}", response.StatusCode, responseContent);
                return [];
            }

            return ParseGeminiResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar texto com IA");
            return [];
        }
    }

    private List<PdfParameterDto> ParseGeminiResponse(string responseContent)
    {
        try
        {
            var response = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var candidates = response.GetProperty("candidates");
            var content = candidates[0].GetProperty("content");
            var parts = content.GetProperty("parts");
            var generatedText = parts[0].GetProperty("text").GetString();

            if (string.IsNullOrEmpty(generatedText))
            {
                _logger.LogWarning("Resposta vazia da API Gemini");
                return [];
            }

            if (generatedText.Contains("```json"))
            {
                var start = generatedText.IndexOf("```json") + 7;
                var end = generatedText.LastIndexOf("```");
                generatedText = generatedText.Substring(start, end - start).Trim();
            }
            
            var jsonStart = generatedText.IndexOf('[');
            var jsonEnd = generatedText.LastIndexOf(']');

            if (jsonStart == -1 || jsonEnd == -1)
            {
                _logger.LogWarning("JSON não encontrado na resposta");
                return [];
            }

            var jsonContent = generatedText.Substring(jsonStart, jsonEnd - jsonStart + 1);
            
            var simpleParams = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var parameters = simpleParams?.Select(p => new PdfParameterDto
            {
                Name = p.GetValueOrDefault("name", ""),
                Value = p.GetValueOrDefault("value", ""),
                Unit = p.GetValueOrDefault("unit", ""),
                Reference = p.GetValueOrDefault("reference", "")
            }).ToList();

            if (parameters != null)
            {
                _logger.LogInformation("Extraídos {Count} parâmetros", parameters.Count);
                return parameters;
            }

            return [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar resposta Gemini: {Response}", responseContent);
            return [];
        }
    }

    public async Task<string> GenerateExamSummaryAsync(Exam exam)
    {
        try
        {
            var parametersText = string.Join("\n", exam.ExamParameters.Select(p => $"• {p.ParameterName}: {p.NumericValue} {p.Unit} (Ref: {p.ReferenceRange})"));
            var prompt = $"Como médico especialista, analise detalhadamente os seguintes resultados de exame:\n\n" +
                $"**EXAME:** {exam.ExamName}\n" +
                $"**DATA:** {exam.ExamDate:dd/MM/yyyy}\n\n" +
                $"**PARÂMETROS:**\n{parametersText}\n\n" +
                $"Forneça uma análise completa incluindo:\n" +
                $"1. **Status Geral:** Normal/Alterado\n" +
                $"2. **Valores Alterados:** Quais estão fora da normalidade\n" +
                $"3. **Possíveis Indicações:** O que esses valores podem indicar\n" +
                $"4. **Pré-diagnóstico:** Suspeitas clínicas baseadas nos resultados\n" +
                $"5. **Recomendações:** Próximos passos ou exames complementares\n\n" +
                $"Seja detalhado e didático, como se estivesse explicando para o paciente.";
            
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    maxOutputTokens = 2000,
                    temperature = 0.4
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Remove("X-goog-api-key");
            _httpClient.DefaultRequestHeaders.Add("X-goog-api-key", _options.ApiKey);
            
            var response = await _httpClient.PostAsync(_options.BaseUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro na API Gemini para resumo: {StatusCode}", response.StatusCode);
                return "Erro ao gerar resumo com IA.";
            }

            var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var candidates = responseJson.GetProperty("candidates");
            var contentProp = candidates[0].GetProperty("content");
            var parts = contentProp.GetProperty("parts");
            var summary = parts[0].GetProperty("text").GetString();

            return ConvertMarkdownToHtml(summary ?? "Não foi possível gerar resumo.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar resumo do exame");
            return "Erro ao gerar resumo com IA.";
        }
    }

    private static string ConvertMarkdownToHtml(string markdown)
    {
        var result = markdown;
        
        // Converter **texto** para <strong>texto</strong>
        while (result.Contains("**"))
        {
            var firstIndex = result.IndexOf("**");
            if (firstIndex == -1) break;
            
            var secondIndex = result.IndexOf("**", firstIndex + 2);
            if (secondIndex == -1) break;
            
            var before = result.Substring(0, firstIndex);
            var content = result.Substring(firstIndex + 2, secondIndex - firstIndex - 2);
            var after = result.Substring(secondIndex + 2);
            
            result = before + "<strong>" + content + "</strong>" + after;
        }
        
        // Converter # Título para <h5>Título</h5>
        var lines = result.Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("### "))
                lines[i] = "<h6>" + lines[i].Substring(4) + "</h6>";
            else if (lines[i].StartsWith("## "))
                lines[i] = "<h5>" + lines[i].Substring(3) + "</h5>";
            else if (lines[i].StartsWith("# "))
                lines[i] = "<h4>" + lines[i].Substring(2) + "</h4>";
        }
        
        return string.Join("<br>", lines);
    }

    private static string RemoveSignature(string text)
    {
        var lines = text.Split('\n').ToList();
        var signaturePatterns = new[] { "Atenciosamente", "Dr(a).", "[Seu Nome]", "[Sua Especialidade]", "Cordialmente" };
        
        for (int i = lines.Count - 1; i >= Math.Max(0, lines.Count - 5); i--)
        {
            if (signaturePatterns.Any(pattern => lines[i].Contains(pattern, StringComparison.OrdinalIgnoreCase)))
            {
                lines.RemoveRange(i, lines.Count - i);
                break;
            }
        }
        
        return string.Join("\n", lines).TrimEnd();
    }

    public async Task<string> GenerateComparisonAnalysisAsync(List<Exam> exams)
    {
        try
        {
            var examsText = new StringBuilder();
            foreach (var exam in exams.OrderBy(e => e.ExamDate))
            {
                examsText.AppendLine($"\n**EXAME {exams.IndexOf(exam) + 1}:** {exam.ExamName}");
                examsText.AppendLine($"**DATA:** {exam.ExamDate:dd/MM/yyyy}");
                examsText.AppendLine($"**LABORATÓRIO:** {exam.Laboratory ?? "Não informado"}");
                examsText.AppendLine($"**PARÂMETROS:**");
                foreach (var param in exam.ExamParameters)
                {
                    examsText.AppendLine($"• {param.ParameterName}: {param.NumericValue ?? param.TextValue} {param.Unit} (Ref: {param.ReferenceRange})");
                }
            }

            var prompt = $"Como médico especialista, compare e analise detalhadamente os seguintes exames:\n" +
                examsText.ToString() +
                $"\n\nForneça uma análise comparativa completa incluindo:\n" +
                $"1. **Evolução Temporal:** Como os valores mudaram ao longo do tempo\n" +
                $"2. **Parâmetros que Melhoraram:** Quais valores evoluíram positivamente\n" +
                $"3. **Parâmetros que Pioraram:** Quais valores se deterioraram\n" +
                $"4. **Parâmetros Estáveis:** Quais se mantiveram constantes\n" +
                $"5. **Análise Clínica:** O que essa evolução pode indicar sobre a saúde do paciente\n" +
                $"6. **Recomendações:** Orientações baseadas na comparação\n\n" +
                $"Seja detalhado e didático, destacando as mudanças mais importantes. NÃO inclua assinatura ou despedida no final.";
            
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    maxOutputTokens = 2500,
                    temperature = 0.4
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Remove("X-goog-api-key");
            _httpClient.DefaultRequestHeaders.Add("X-goog-api-key", _options.ApiKey);
            
            var response = await _httpClient.PostAsync(_options.BaseUrl, content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Erro na API Gemini para comparação: {StatusCode}", response.StatusCode);
                return "Erro ao gerar análise comparativa com IA.";
            }

            var responseJson = JsonSerializer.Deserialize<JsonElement>(responseContent);
            var candidates = responseJson.GetProperty("candidates");
            var contentProp = candidates[0].GetProperty("content");
            var parts = contentProp.GetProperty("parts");
            var analysis = parts[0].GetProperty("text").GetString() ?? "Não foi possível gerar análise comparativa.";

            analysis = RemoveSignature(analysis);
            return ConvertMarkdownToHtml(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar análise comparativa");
            return "Erro ao gerar análise comparativa com IA.";
        }
    }

    public void Dispose() => _httpClient?.Dispose();
}