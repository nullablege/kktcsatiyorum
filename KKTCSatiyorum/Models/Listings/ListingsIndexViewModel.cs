using EntityLayer.DTOs.Public;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KKTCSatiyorum.Models.Listings
{
    public class ListingsIndexViewModel
    {
        public ListingSearchQuery Query { get; set; } = new();
        public PagedResult<ListingCardDto> Listings { get; set; } = null!;
        
        public IEnumerable<SelectListItem> KategoriOptions { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> SortOptions { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
