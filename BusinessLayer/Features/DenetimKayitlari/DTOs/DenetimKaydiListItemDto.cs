using System;

namespace BusinessLayer.Features.DenetimKayitlari.DTOs
{
    public class DenetimKaydiListItemDto
    {
        public long Id { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public string Eylem { get; set; } = default!;
        public string VarlikAdi { get; set; } = default!;
        public string VarlikId { get; set; } = default!;
        public string? KullaniciId { get; set; }
        public string? KullaniciEmail { get; set; }
        public string? IpAdresi { get; set; }
    }
}
