using EntityLayer.Enums;
using EntityLayer.DTOs.Public;

namespace BusinessLayer.Features.Ilanlar.DTOs
{
    public sealed record MemberListingDetailDto(
        int Id,
        int KategoriId,
        string Baslik,
        string Aciklama,
        decimal Fiyat,
        ParaBirimi ParaBirimi,
        string? Sehir,
        string? Ilce,
        decimal? Enlem,
        decimal? Boylam,
        List<EditAttributeDto> Attributes,
        List<PhotoDto> Photos,
        IlanDurumu Durum,
        string? RedNedeni,
        string SeoSlug,
        DateTime OlusturmaTarihi,
        DateTime? YayinTarihi
    );
}
