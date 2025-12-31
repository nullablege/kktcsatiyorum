using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace KKTCSatiyorum.Hubs
{
    [Authorize]
    public class NotificationsHub : Hub
    {

    }
}
