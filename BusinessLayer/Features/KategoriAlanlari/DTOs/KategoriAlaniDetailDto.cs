using EntityLayer.Enums;

namespace BusinessLayer.Features.KategoriAlanlari.DTOs
{
    public sealed record KategoriAlaniDetailDto(
        int Id,
        int KategoriId,
        string KategoriAdi,
        string Ad,
        string Anahtar,
        VeriTipi VeriTipi,
        bool ZorunluMu,
        bool FiltrelenebilirMi,
        int SiraNo,
        bool AktifMi,
        List<KategoriAlaniSecenegiDto> Secenekler
    );
}
