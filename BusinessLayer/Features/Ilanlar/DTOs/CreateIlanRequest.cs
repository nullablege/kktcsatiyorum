using EntityLayer.Enums;

namespace BusinessLayer.Features.Ilanlar.DTOs
{
    public sealed record CreateIlanRequest(
        int KategoriId,
        string Baslik,
        string Aciklama,
        decimal Fiyat,
        ParaBirimi ParaBirimi,
        string Sehir,
        string? Ilce,
        decimal? Enlem,
        decimal? Boylam,
        List<AttributeValueInput> Attributes,
        List<string> PhotoPaths
    );

}
