using BusinessLayer.Common.Results;
using BusinessLayer.Features.KategoriAlanlari.DTOs;

namespace BusinessLayer.Features.KategoriAlanlari.Services
{
    public interface IKategoriAlaniService
    {
        Task<Result<int>> CreateAsync(CreateKategoriAlaniRequest request, CancellationToken ct = default);
        Task<Result> UpdateAsync(UpdateKategoriAlaniRequest request, CancellationToken ct = default);
        Task<Result> SoftDeleteAsync(int id, CancellationToken ct = default);
        Task<Result<IReadOnlyList<KategoriAlaniListItemDto>>> GetListByKategoriAsync(int kategoriId, CancellationToken ct = default);
        Task<Result<KategoriAlaniDetailDto>> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
