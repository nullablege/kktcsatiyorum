using System;

namespace BusinessLayer.Common.DTOs
{
    public class NotificationPushedDto
    {
        public int Id { get; set; }
        public string Tur { get; set; } = string.Empty;
        public string VeriJson { get; set; } = string.Empty;
        public DateTime OlusturmaTarihi { get; set; }
        public string Mesaj { get; set; } = string.Empty;
    }
}
