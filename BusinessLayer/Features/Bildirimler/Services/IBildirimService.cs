using BusinessLayer.Common.Results;
using BusinessLayer.Features.Bildirimler.DTOs;
using EntityLayer.DTOs.Public;

namespace BusinessLayer.Features.Bildirimler.Services
{
    public interface IBildirimService
    {
        Task<Result<PagedResult<MyNotificationDto>>> GetMyNotificationsAsync(string userId, int page, int pageSize, CancellationToken ct = default);
        Task<Result> MarkAsReadAsync(int notificationId, string userId, CancellationToken ct = default);
    }
}
