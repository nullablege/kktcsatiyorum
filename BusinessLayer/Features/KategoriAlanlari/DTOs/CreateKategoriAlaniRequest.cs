using EntityLayer.Enums;

namespace BusinessLayer.Features.KategoriAlanlari.DTOs
{
    public sealed record CreateKategoriAlaniRequest(
        int KategoriId,
        string Ad,
        string Anahtar,
        VeriTipi VeriTipi,
        bool ZorunluMu,
        bool FiltrelenebilirMi,
        int SiraNo,
        List<string>? Secenekler
    );
}
