using DataAccessLayer.Projections;
using EntityLayer.DTOs.Public;
using EntityLayer.Entities;
using System.Linq.Expressions;

namespace DataAccessLayer.Abstract
{
    public interface IIlanDal : IGenericRepository<Ilan>
    {
        Task<List<Ilan>> GetIlanListWithCategoryAndPhotosAsync(Expression<Func<Ilan, bool>>? filter = null, CancellationToken ct = default);
        Task<Ilan> GetIlanByIdWithDetailsAsync(int id, CancellationToken ct = default);
        Task<List<Ilan>> GetIlanListByUserIdAsync(string userId, CancellationToken ct = default);
        
        Task<PagedResult<ListingCardDto>> SearchPublicAsync(ListingSearchQuery query, CancellationToken ct = default);
        Task<ListingDetailDto?> GetPublicDetailBySlugAsync(string slug, CancellationToken ct = default);
        Task<PagedResult<PendingListingProjection>> GetPendingApprovalsAsync(int page, int pageSize, CancellationToken ct = default);
        Task<PagedResult<MyListingProjection>> GetUserListingsAsync(string userId, int page, int pageSize, CancellationToken ct = default);
    }
}
