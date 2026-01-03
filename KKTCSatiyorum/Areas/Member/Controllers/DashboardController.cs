using BusinessLayer.Features.Member.Services;
using EntityLayer.Constants;
using KKTCSatiyorum.Areas.Member.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KKTCSatiyorum.Areas.Member.Controllers
{
    [Area("Member")]
    [Authorize(Roles = RoleNames.User)]
    public class DashboardController : Controller
    {
        private readonly IMemberService _memberService;

        public DashboardController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var result = await _memberService.GetDashboardStatsAsync(userId);
            if (!result.IsSuccess || result.Data == null)
            {
                return View(new DashboardIndexViewModel());
            }

            var model = new DashboardIndexViewModel
            {
                Stats = result.Data
            };

            return View(model);
        }
    }
}
