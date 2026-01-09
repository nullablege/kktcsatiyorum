namespace KKTCSatiyorum.Models
{
    public class StateMessageViewModel
    {
        public string Icon { get; set; } = "bi-info-circle";
        public string IconClass { get; set; } = "text-muted";
        public string Title { get; set; } = "Bilgi";
        public string Message { get; set; } = "İşlem hakkında bilgi bulunamadı.";

        // Primary Action
        public string? ActionText { get; set; }
        public string? ActionController { get; set; }
        public string? ActionAction { get; set; }
        public string ActionArea { get; set; } = "";
        public string ActionBtnClass { get; set; } = "btn-primary";
        public string? ActionIcon { get; set; }

        // Secondary Action
        public string? SecActionText { get; set; }
        public string? SecActionController { get; set; }
        public string? SecActionAction { get; set; }
        public string SecActionArea { get; set; } = "";
    }
}
