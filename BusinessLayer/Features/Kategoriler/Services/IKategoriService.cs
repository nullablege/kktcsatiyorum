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
    }
}
