using EntityLayer.Enums;

namespace DataAccessLayer.Projections
{
    public sealed record MyListingProjection(
        int Id,
        string Baslik,
        string SeoSlug,
        decimal Fiyat,
        ParaBirimi ParaBirimi,
        IlanDurumu Durum,
        string? RedNedeni,
        DateTime OlusturmaTarihi,
        DateTime? YayinTarihi,
        string KategoriAdi,
        string? KapakFotoUrl
    );
}
