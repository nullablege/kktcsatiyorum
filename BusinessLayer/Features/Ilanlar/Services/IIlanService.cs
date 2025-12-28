using BusinessLayer.Common.Results;
using BusinessLayer.Features.Ilanlar.DTOs;

namespace BusinessLayer.Features.Ilanlar.Services
{
    public interface IIlanService
    {
        Task<Result<int>> CreateAsync(CreateIlanRequest request, string userId, CancellationToken ct = default);
    }
}
