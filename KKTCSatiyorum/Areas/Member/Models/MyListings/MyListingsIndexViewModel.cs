using KKTCSatiyorum.Models.Common;

namespace KKTCSatiyorum.Areas.Member.Models.MyListings
{
    public class MyListingsIndexViewModel
    {
        public PagedViewModel<MyListingRowViewModel> Listings { get; set; } = new();
    }
}
