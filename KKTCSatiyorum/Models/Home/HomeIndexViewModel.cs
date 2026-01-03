using EntityLayer.DTOs.Public;

namespace KKTCSatiyorum.Models.Home
{
    public class HomeIndexViewModel
    {
        public IReadOnlyList<ListingCardDto> FeaturedListings { get; set; } = Array.Empty<ListingCardDto>();
        public IReadOnlyList<ListingCardDto> LatestListings { get; set; } = Array.Empty<ListingCardDto>();
    }
}
