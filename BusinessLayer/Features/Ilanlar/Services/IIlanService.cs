using BusinessLayer.Common.Results;
using BusinessLayer.Features.Ilanlar.DTOs;
using EntityLayer.DTOs.Public;

namespace BusinessLayer.Features.Ilanlar.Services
{
    public interface IIlanService
    {
        Task<Result<int>> CreateAsync(CreateIlanRequest request, string userId, CancellationToken ct = default);
        Task<Result<PagedResult<ListingCardDto>>> SearchAsync(ListingSearchQuery query, CancellationToken ct = default);
        Task<Result<ListingDetailDto>> GetPublicDetailBySlugAsync(string slug, CancellationToken ct = default);
        
        Task<Result<PagedResult<PendingListingDto>>> GetPendingApprovalsAsync(int page, int pageSize, CancellationToken ct = default);
        Task<Result> ApproveAsync(int listingId, string adminUserId, CancellationToken ct = default);
        Task<Result> RejectAsync(int listingId, string adminUserId, string redNedeni, CancellationToken ct = default);
    }
}
