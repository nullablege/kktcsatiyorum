using KKTCSatiyorum.Models.Common;

namespace KKTCSatiyorum.Areas.Member.Models.Notifications
{
    public class MyNotificationsIndexViewModel
    {
        public PagedViewModel<MyNotificationRowViewModel> Notifications { get; set; } = new();
    }
}
