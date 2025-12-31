using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Common.Abstractions;
using BusinessLayer.Common.DTOs;

namespace BusinessLayer.Common.Services
{
    public class NoOpNotificationPublisher : INotificationPublisher
    {
        public Task PublishAsync(string userId, NotificationPushedDto payload, CancellationToken ct)
        {
            // No-op
            return Task.CompletedTask;
        }
    }
}
