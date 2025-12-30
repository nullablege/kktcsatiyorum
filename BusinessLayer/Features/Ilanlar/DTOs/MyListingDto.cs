using EntityLayer.Enums;

namespace BusinessLayer.Features.Ilanlar.DTOs
{
    public sealed record MyListingDto(
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
