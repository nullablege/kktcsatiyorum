using EntityLayer.Enums;

namespace BusinessLayer.Features.Ilanlar.DTOs
{
    public sealed record PendingListingDto(
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
