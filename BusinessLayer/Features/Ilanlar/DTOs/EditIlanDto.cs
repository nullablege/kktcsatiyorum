using EntityLayer.Enums;
using EntityLayer.DTOs.Public;

namespace BusinessLayer.Features.Ilanlar.DTOs
{
    public record EditIlanDto(
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
        List<PhotoDto> Photos
    );


    public record EditAttributeDto(
        int KategoriAlaniId,
        string Ad,
        string? Value, // Raw value for input binding
        VeriTipi VeriTipi,
        List<SecenekDto>? Secenekler // Options if it's a select
    );
}
