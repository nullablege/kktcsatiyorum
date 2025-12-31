using BusinessLayer.Common.Abstractions;
using BusinessLayer.Common.DTOs;
using KKTCSatiyorum.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace KKTCSatiyorum.Services
{
    public class SignalRNotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public SignalRNotificationPublisher(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PublishAsync(string userId, NotificationPushedDto payload, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(userId)) return;

            // "notificationReceived" is the method name client listens to
            await _hubContext.Clients.User(userId).SendAsync("notificationReceived", payload, ct);
        }
    }
}
