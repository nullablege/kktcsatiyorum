
using BusinessLayer.Features.Member.DTOs;
using BusinessLayer.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Member.Services
{
    public interface IMemberService
    {
        Task<Result<MemberDashboardStatsDto>> GetDashboardStatsAsync(string userId, CancellationToken ct = default);
        Task<Result<MyProfileDto>> GetMyProfileAsync(string userId, CancellationToken ct = default);
        Task<Result> UpdateMyProfileAsync(string userId, UpdateProfileRequest request, CancellationToken ct = default);
    }
}
