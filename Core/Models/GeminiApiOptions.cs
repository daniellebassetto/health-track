namespace HealthTrack.Core.Models;

public class GeminiApiOptions
{
    public const string SectionName = "GeminiApi";

    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 500;
    public double Temperature { get; set; } = 0.0;
}