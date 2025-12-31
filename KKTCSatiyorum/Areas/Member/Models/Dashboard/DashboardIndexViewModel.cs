
using BusinessLayer.Features.Member.DTOs;

namespace KKTCSatiyorum.Areas.Member.Models.Dashboard
{
    public class DashboardIndexViewModel
    {
        public MemberDashboardStatsDto Stats { get; set; } = new();
    }
}
