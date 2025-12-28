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
        List<AttributeValueInput> Attributes,
        List<string> PhotoPaths
    );

    public sealed record AttributeValueInput(
        int KategoriAlaniId,
        string? Value
    );
}
