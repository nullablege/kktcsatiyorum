
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Member.DTOs
{
    public class MemberDashboardStatsDto
    {
        public int ActiveListingCount { get; set; }
        public int PendingListingCount { get; set; }
        public int FavoriteCount { get; set; }
        public int UnreadNotificationCount { get; set; }
    }
}
