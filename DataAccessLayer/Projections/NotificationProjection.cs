using EntityLayer.Enums;

namespace DataAccessLayer.Projections
{
    public class NotificationProjection
    {
        public int Id { get; set; }
        public BildirimTuru Tur { get; set; }
        public string Mesaj { get; set; } = "";
        public string? VeriJson { get; set; }
        public bool OkunduMu { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
    }
}
