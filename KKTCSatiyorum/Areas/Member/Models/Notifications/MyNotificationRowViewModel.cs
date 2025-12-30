using EntityLayer.Enums;

namespace KKTCSatiyorum.Areas.Member.Models.Notifications
{
    public class MyNotificationRowViewModel
    {
        public int Id { get; set; }
        public BildirimTuru Tur { get; set; }
        public string? VeriJson { get; set; }
        public bool OkunduMu { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
    }
}
