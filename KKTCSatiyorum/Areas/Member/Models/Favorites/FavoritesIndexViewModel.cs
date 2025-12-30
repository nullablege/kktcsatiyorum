using KKTCSatiyorum.Models.Common;

namespace KKTCSatiyorum.Areas.Member.Models.Favorites
{
    public class FavoritesIndexViewModel
    {
        public PagedViewModel<FavoriteListingRowViewModel> Listings { get; set; } = new();
    }
}
