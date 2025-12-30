using EntityLayer.Enums;

namespace BusinessLayer.Features.Bildirimler.DTOs
{
    public class MyNotificationDto
    {
        public int Id { get; set; }
        public BildirimTuru Tur { get; set; }
        public string? VeriJson { get; set; }
        public bool OkunduMu { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
    }
}
