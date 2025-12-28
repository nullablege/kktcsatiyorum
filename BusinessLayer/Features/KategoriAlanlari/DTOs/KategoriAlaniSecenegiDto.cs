namespace BusinessLayer.Features.KategoriAlanlari.DTOs
{
    public sealed record KategoriAlaniSecenegiDto(
        int Id,
        string Deger,
        int SiraNo,
        bool AktifMi
    );
}
