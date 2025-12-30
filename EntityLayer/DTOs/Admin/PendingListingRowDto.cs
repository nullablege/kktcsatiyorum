using EntityLayer.Enums;

namespace EntityLayer.DTOs.Admin
{
    public sealed record PendingListingRowDto(
        int Id,
        string Baslik,
        string SeoSlug,
        decimal Fiyat,
        ParaBirimi ParaBirimi,
        string? Sehir,
        DateTime OlusturmaTarihi,
        string KategoriAdi,
        string SahipKullaniciId,
        string SahipAdSoyad,
        string? SahipEmail,
        string? KapakFotoUrl
    );
}
