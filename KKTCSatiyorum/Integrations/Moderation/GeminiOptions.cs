namespace KKTCSatiyorum.Integrations.Moderation
{
    public class GeminiOptions
    {
        public bool Enabled { get; set; }
        public string? ApiKey { get; set; }
        public string? Endpoint { get; set; }
        public int TimeoutSeconds { get; set; } = 5;
    }
}
