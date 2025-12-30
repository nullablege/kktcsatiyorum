using EntityLayer.DTOs.Public;

namespace KKTCSatiyorum.Areas.Admin.Models.Moderasyon
{
    public class ModerasyonIndexViewModel
    {
        public PagedResult<PendingListingRowViewModel> Listings { get; set; } =
            new PagedResult<PendingListingRowViewModel>(Array.Empty<PendingListingRowViewModel>(), 0, 1, 10);
    }
}
