using EntityLayer.Entities;

namespace DataAccessLayer.Abstract
{
    public interface IBildirimDal : IGenericRepository<Bildirim>
    {
        Task<List<Bildirim>> GetOkunmamisBildirimlerByUserIdAsync(string userId, CancellationToken ct = default);
        Task<EntityLayer.DTOs.Public.PagedResult<Projections.NotificationProjection>> GetUserNotificationsAsync(string userId, int page, int pageSize, CancellationToken ct = default);
    }
}
