using BusinessLayer.Common.Results;
using BusinessLayer.Features.Favoriler.DTOs;
using EntityLayer.DTOs.Public;
using System.Threading;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Favoriler.Services
{
    public interface IFavoriService
    {
        Task<Result<FavoriteToggleResultDto>> ToggleAsync(int ilanId, string userId, CancellationToken ct = default);
        Task<Result<PagedResult<FavoriteListingDto>>> GetMyFavoritesAsync(string userId, int page, int pageSize, CancellationToken ct = default);
        Task<Result<bool>> IsFavoriteAsync(int ilanId, string userId, CancellationToken ct = default);
    }
}
