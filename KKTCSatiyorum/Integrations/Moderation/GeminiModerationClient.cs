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
            if (string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(_options.Endpoint))
            {
                _logger.LogWarning("Gemini configuration missing (ApiKey or Endpoint). Skipping moderation.");
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
                    return new ModerationDecision 
                    { 
                         IsAllowed = false, 
                         ReasonCode = "UNAVAILABLE", 
                         ReasonMessage = "Moderasyon servisine ulaşılamıyor." 
                    };
                }

                var responseString = await response.Content.ReadAsStringAsync(ct);
                var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseString);
                
                // Safe access using LINQ FirstOrDefault
                var textResponse = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;

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
                return new ModerationDecision 
                { 
                        IsAllowed = false, 
                        ReasonCode = "UNAVAILABLE", 
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
