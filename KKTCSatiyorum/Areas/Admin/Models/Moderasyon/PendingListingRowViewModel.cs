using EntityLayer.Enums;

namespace KKTCSatiyorum.Areas.Admin.Models.Moderasyon
{
    public class PendingListingRowViewModel
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string SeoSlug { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public ParaBirimi ParaBirimi { get; set; }
        public string? Sehir { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public string KategoriAdi { get; set; } = string.Empty;
        public string SahipAdSoyad { get; set; } = string.Empty;
        public string? SahipEmail { get; set; }
        public string? KapakFotoUrl { get; set; }
    }
}
