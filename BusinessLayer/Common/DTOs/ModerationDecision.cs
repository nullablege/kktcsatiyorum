namespace BusinessLayer.Common.DTOs
{
    public class ModerationDecision
    {
        public bool IsAllowed { get; set; }
        public string? ReasonCode { get; set; }
        public string? ReasonMessage { get; set; }

        public static ModerationDecision Allowed() => new() { IsAllowed = true };
        public static ModerationDecision Blocked(string reasonCode, string message) => new() 
        { 
            IsAllowed = false, 
            ReasonCode = reasonCode, 
            ReasonMessage = message 
        };
    }
}
