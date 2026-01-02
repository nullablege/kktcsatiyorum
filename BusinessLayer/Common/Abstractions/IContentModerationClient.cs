using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Common.DTOs;

namespace BusinessLayer.Common.Abstractions
{
    public interface IContentModerationClient
    {
        Task<ModerationDecision> ModerateListingAsync(string title, string description, CancellationToken ct);
    }
}
