using BusinessLayer.Features.Kategoriler.DTOs;
using EntityLayer.DTOs.Public;

namespace KKTCSatiyorum.Models.Listings
{
    public class ListingsIndexViewModel
    {
        public ListingSearchQuery Query { get; set; } = new();
        public PagedResult<ListingCardDto> Listings { get; set; } = null!;
        public IReadOnlyList<KategoriDropdownItemDto> Kategoriler { get; set; } 
            = new List<KategoriDropdownItemDto>();
    }
}
