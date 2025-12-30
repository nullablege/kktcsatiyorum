using System;

namespace BusinessLayer.Features.Favoriler.DTOs
{
    public class FavoriteListingDto
    {
        public int IlanId { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string SeoSlug { get; set; } = string.Empty;
        public decimal Fiyat { get; set; }
        public string Sehir { get; set; } = string.Empty;
        public DateTime OlusturmaTarihi { get; set; }
        public string? KapakFotoUrl { get; set; }
        public string KategoriAdi { get; set; } = string.Empty;
    }
}
