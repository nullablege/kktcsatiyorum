using EntityLayer.DTOs.Public;
using EntityLayer.Enums;

namespace KKTCSatiyorum.Areas.Member.Models
{
    public class MyListingsIndexViewModel
    {
        public PagedResult<MyListingRowViewModel> Listings { get; set; } = new(new List<MyListingRowViewModel>(), 0, 1, 10);
    }

    public class MyListingRowViewModel
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = "";
        public string SeoSlug { get; set; } = "";
        public decimal Fiyat { get; set; }
        public ParaBirimi ParaBirimi { get; set; }
        public IlanDurumu Durum { get; set; }
        public string? RedNedeni { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? YayinTarihi { get; set; }
        public string KategoriAdi { get; set; } = "";
        public string? KapakFotoUrl { get; set; }
    }
}
