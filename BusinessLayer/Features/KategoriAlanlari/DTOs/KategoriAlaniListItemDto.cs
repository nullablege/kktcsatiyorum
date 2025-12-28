using EntityLayer.Enums;

namespace BusinessLayer.Features.KategoriAlanlari.DTOs
{
    public sealed record KategoriAlaniListItemDto(
        int Id,
        string Ad,
        string Anahtar,
        VeriTipi VeriTipi,
        bool ZorunluMu,
        bool FiltrelenebilirMi,
        int SiraNo,
        bool AktifMi,
        int SecenekSayisi
    );
}
