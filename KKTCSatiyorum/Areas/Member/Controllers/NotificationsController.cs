using BusinessLayer.Features.Bildirimler.Services;
using BusinessLayer.Features.Bildirimler.DTOs;
using EntityLayer.Enums;
using KKTCSatiyorum.Areas.Member.Models.Notifications;
using KKTCSatiyorum.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KKTCSatiyorum.Areas.Member.Controllers
{
    public class NotificationsController : MemberBaseController
    {
        private readonly IBildirimService _bildirimService;

        public NotificationsController(IBildirimService bildirimService)
        {
            _bildirimService = bildirimService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            const int pageSize = 15;
            var result = await _bildirimService.GetMyNotificationsAsync(userId, page, pageSize, ct);

            var vm = new MyNotificationsIndexViewModel();

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "Bildirimler yüklenirken bir hata oluştu.";
                return View(vm);
            }

            if (result.Data != null)
            {
                vm.Notifications = new PagedViewModel<MyNotificationRowViewModel>
                {
                    Items = result.Data.Items.Select(x => new MyNotificationRowViewModel
                    {
                        Id = x.Id,
                        Tur = x.Tur,
                        Mesaj = x.Mesaj,
                        VeriJson = x.VeriJson,
                        OkunduMu = x.OkunduMu,
                        OlusturmaTarihi = x.OlusturmaTarihi
                    }).ToList(),
                    TotalCount = result.Data.TotalCount,
                    Page = result.Data.Page,
                    PageSize = result.Data.PageSize
                };
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int id, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _bildirimService.MarkAsReadAsync(id, userId, ct);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "Bildirim güncellenemedi.";
            }

            var referer = Request.Headers["Referer"].ToString();
            return Url.IsLocalUrl(referer) ? Redirect(referer) : RedirectToAction(nameof(Index));
        }
    }
}
