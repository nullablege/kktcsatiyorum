using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Common.Abstractions;
using BusinessLayer.Common.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KKTCSatiyorum.Integrations.Moderation
{
    public class GeminiModerationClient : IContentModerationClient
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiOptions _options;
        private readonly ILogger<GeminiModerationClient> _logger;

        public GeminiModerationClient(HttpClient httpClient, IOptions<GeminiOptions> options, ILogger<GeminiModerationClient> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<ModerationDecision> ModerateListingAsync(string title, string description, CancellationToken ct)
        {
            // Fallback check
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                _logger.LogWarning("Gemini API Key is missing. Skipping moderation.");
                return ModerationDecision.Allowed();
            }

            var prompt = $@"
You are a content moderation AI. Your task is to review the following classified ad listing for a platform in North Cyprus (TRNC/KKTC).
The content should be safe, not illegal, not spam, and suitable for a general audience.
Strictly prohibit: hate speech, explicit violence, sexual content, illegal drugs/weapons, and obvious spam/scams.

Title: {title}
Description: {description}

Respond ONLY with a valid JSON object in the following format:
{{
  ""isAllowed"": true/false,
  ""reasonCode"": ""string_code"" (e.g. ""HATE_SPEECH"", ""SEXUAL"", ""SPAM"", ""SAFE""),
  ""reasonMessage"": ""Human readable short explanation (turkish preferred)""
}}
";

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
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var url = $"{_options.Endpoint}?key={_options.ApiKey}";

            try
            {
                var response = await _httpClient.PostAsync(url, content, ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini API call failed with status code: {StatusCode}", response.StatusCode);
                    return ModerationDecision.Allowed(); // Open on error principle, or Block based on policy. User asked for "Unavailable" error code but also said "fallback allow". Reviewing requirement: "Moderasyon servisi down -> 'Şu an moderasyon servisine ulaşılamıyor' gibi kontrollü mesaj". So I should probably return blocked/fail or a specific error?
                    // Requirement says: "Fallback (çok önemli): Enabled=false veya ApiKey boşsa NoOpModerationClient kullan (her şeye allow)."
                    // Requirement also says: "Timeout + hata yönetimi olacak (servis down olursa kontrollü mesaj)"
                    // And: "Uygunsuz metin -> create/edit engelleniyor. Uygun metin -> akış bozulmuyor. Moderasyon servisi down -> 'Şu an moderasyon servisine ulaşılamıyor' gibi kontrollü mesaj"
                    
                    // So if service is down (HTTP error), I should probably throwing or returning a decision that causes the controller to show a message.
                    // Let's throw a specific exception or return a decision with 'Unavailable' reason if strictly following "controlled message".
                    // However, to keep it simple and safe for now, if it fails, I might block it temporarily saying service unavailable.
                    
                    return new ModerationDecision 
                    { 
                         IsAllowed = false, 
                         ReasonCode = "SERVICE_UNAVAILABLE", 
                         ReasonMessage = "Moderasyon servisine ulaşılamıyor." 
                    };
                }

                var responseString = await response.Content.ReadAsStringAsync(ct);
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString);
                
                var textResponse = geminiResponse?.Candidates?[0]?.Content?.Parts?[0]?.Text;

                if (string.IsNullOrWhiteSpace(textResponse))
                {
                     _logger.LogError("Empty response from Gemini.");
                     return ModerationDecision.Allowed();
                }

                // Clean up markdown code blocks if present
                textResponse = textResponse.Replace("```json", "").Replace("```", "").Trim();

                var decision = JsonSerializer.Deserialize<ModerationDecision>(textResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return decision ?? ModerationDecision.Allowed();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Gemini API");
                 // If actual exception (timeout etc), return unavailable so user knows.
                return new ModerationDecision 
                { 
                        IsAllowed = false, 
                        ReasonCode = "SERVICE_ERROR", 
                        ReasonMessage = "Moderasyon servisi hatası." 
                };
            }
        }
        
        // Internal DTOs for Gemini Response
        private class GeminiResponse
        {
            [JsonPropertyName("candidates")]
            public Candidate[]? Candidates { get; set; }
        }

        private class Candidate
        {
             [JsonPropertyName("content")]
            public Content? Content { get; set; }
        }

        private class Content
        {
            [JsonPropertyName("parts")]
            public Part[]? Parts { get; set; }
        }

        private class Part
        {
            [JsonPropertyName("text")]
            public string? Text { get; set; }
        }
    }
}
