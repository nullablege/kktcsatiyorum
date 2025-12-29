namespace EntityLayer.DTOs.Public
{
    public sealed record ListingSearchQuery
    {
        public string? Q { get; init; }
        public int? KategoriId { get; init; }
        public decimal? MinFiyat { get; init; }
        public decimal? MaxFiyat { get; init; }
        public string? Sehir { get; init; }
        public string? Sort { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 12;
        public Dictionary<int, string>? EavFilters { get; init; }
    }
}
