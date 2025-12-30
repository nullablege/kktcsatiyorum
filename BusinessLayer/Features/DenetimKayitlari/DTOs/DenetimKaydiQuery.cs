namespace BusinessLayer.Features.DenetimKayitlari.DTOs
{
    public class DenetimKaydiQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
        public DateTime? BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public string? Eylem { get; set; }
        public string? VarlikAdi { get; set; }
        public string? KullaniciId { get; set; }
    }
}
