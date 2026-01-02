using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.Common.Abstractions;
using BusinessLayer.Common.DTOs;

namespace KKTCSatiyorum.Integrations.Moderation
{
    public class NoOpModerationClient : IContentModerationClient
    {
        public Task<ModerationDecision> ModerateListingAsync(string title, string description, CancellationToken ct)
        {
            return Task.FromResult(ModerationDecision.Allowed());
        }
    }
}
