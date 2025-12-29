using EntityLayer.Enums;

namespace EntityLayer.DTOs.Public
{
    public sealed record ListingCardDto(
        int Id,
        string Slug,
        string Baslik,
        decimal Fiyat,
        ParaBirimi ParaBirimi,
        string? Sehir,
        string? KapakFotoUrl,
        DateTime OlusturmaTarihi,
        string? KategoriAd = null
    );
}
