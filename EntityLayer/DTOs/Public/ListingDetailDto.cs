using EntityLayer.Enums;

namespace EntityLayer.DTOs.Public
{
    public sealed record ListingDetailDto(
        int Id,
        string Slug,
        string Baslik,
        string? Aciklama,
        decimal Fiyat,
        ParaBirimi ParaBirimi,
        string? Sehir,
        DateTime OlusturmaTarihi,
        string? SaticiAd,
        List<PhotoDto> Fotograflar,
        List<AttributeValueDisplayDto> Attributes
    );

    public sealed record PhotoDto(string Url, bool KapakMi, int SiraNo);

    public sealed record AttributeValueDisplayDto(string Label, string Value);
}
