using EntityLayer.Enums;

namespace BusinessLayer.Features.Ilanlar.DTOs
{
    public record UpdateIlanRequest(
        int KategoriId,
        string Baslik,
        string Aciklama,
        decimal Fiyat,
        ParaBirimi ParaBirimi,
        string? Sehir,
        string? Ilce,
        decimal? Enlem,
        decimal? Boylam,
        List<AttributeValueInput> Attributes
        // Photos are handled separately or in a future iteration
    );
}
