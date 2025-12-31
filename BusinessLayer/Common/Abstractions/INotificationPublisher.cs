using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Common.DTOs;

namespace BusinessLayer.Common.Abstractions
{
    public interface INotificationPublisher
    {
        Task PublishAsync(string userId, NotificationPushedDto payload, CancellationToken ct);
    }
}
