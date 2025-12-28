using EntityLayer.Enums;

namespace BusinessLayer.Features.KategoriAlanlari.DTOs
{
    public sealed record UpdateKategoriAlaniRequest(
        int Id,
        string Ad,
        string Anahtar,
        VeriTipi VeriTipi,
        bool ZorunluMu,
        bool FiltrelenebilirMi,
        int SiraNo,
        bool AktifMi
    );
}
