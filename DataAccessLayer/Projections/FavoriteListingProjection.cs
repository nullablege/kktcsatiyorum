using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Projections
{
    public class FavoriteListingProjection
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
