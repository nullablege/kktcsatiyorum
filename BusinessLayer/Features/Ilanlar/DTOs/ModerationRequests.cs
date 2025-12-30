namespace BusinessLayer.Features.Ilanlar.DTOs
{
    public sealed record ApproveListingRequest(int ListingId, string AdminUserId);
    public sealed record RejectListingRequest(int ListingId, string AdminUserId, string RedNedeni);
}
