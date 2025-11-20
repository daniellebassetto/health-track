using System.Text.Json.Serialization;

namespace HealthTrack.Core.Models;

public class GroqRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("messages")]
    public List<GeminiMessage> Messages { get; set; } = new();

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }
}

public class GeminiMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}

public class GroqResponse
{
    [JsonPropertyName("choices")]
    public List<GroqChoice> Choices { get; set; } = new();

    [JsonPropertyName("error")]
    public GroqError? Error { get; set; }
}

public class GroqChoice
{
    [JsonPropertyName("message")]
    public GeminiMessage Message { get; set; } = new();
}

public class GroqError
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}