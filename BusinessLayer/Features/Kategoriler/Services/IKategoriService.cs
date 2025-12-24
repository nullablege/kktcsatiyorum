using BusinessLayer.Common.Results;
using BusinessLayer.Features.Kategoriler.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Kategoriler.Services
{
    public interface IKategoriService
    {
        Task<Result<CreateKategoriResponse>> CreateAsync(CreateKategoriRequest request, CancellationToken ct = default);

        Task<Result<IReadOnlyList<KategoriListItemDto>>> GetListAsync( CancellationToken ct = default);
        Task<Result<IReadOnlyList<KategoriDropdownItemDto>>> GetForDropdownAsync(CancellationToken ct = default);
        Task<Result<KategoriDetailDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<UpdateKategoriResponse>> UpdateAsync(UpdateKategoriRequest request, CancellationToken ct = default);
        Task<Result<SoftDeleteKategoriResponse>> SoftDeleteAsync(SoftDeleteKategoriRequest request, CancellationToken ct = default);
        Task<Result<IReadOnlyList<KategoriListItemDto>>> GetChildrensAsync(int UstKategorıId, CancellationToken ct = default);

    }
}
