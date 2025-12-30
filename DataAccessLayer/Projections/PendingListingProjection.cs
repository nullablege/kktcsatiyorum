using EntityLayer.Enums;

namespace DataAccessLayer.Projections
{
    public sealed record PendingListingProjection(
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
