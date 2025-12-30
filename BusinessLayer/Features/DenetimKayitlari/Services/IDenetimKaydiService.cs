using BusinessLayer.Common.Results;
using BusinessLayer.Features.DenetimKayitlari.DTOs;

using EntityLayer.DTOs.Public;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Features.DenetimKayitlari.Services
{
    public interface IDenetimKaydiService
    {
        Task<Result<PagedResult<DenetimKaydiListItemDto>>> GetPagedAsync(DenetimKaydiQuery query, CancellationToken ct);
        Task<Result> LogAsync(string eylem, string varlikAdi, string varlikId, string? detayJson, string? ipAdresi, string? kullaniciId, CancellationToken ct);
    }
}
