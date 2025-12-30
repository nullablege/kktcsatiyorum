using BusinessLayer.Features.Ilanlar.Services;
using BusinessLayer.Features.DenetimKayitlari.Services;
using KKTCSatiyorum.Areas.Admin.Models.Moderasyon;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KKTCSatiyorum.Areas.Admin.Controllers
{
    public class ModerasyonController : AdminBaseController
    {
        private readonly IIlanService _ilanService;

        private readonly IDenetimKaydiService _auditService;

        public ModerasyonController(IIlanService ilanService, IDenetimKaydiService auditService)
        {
            _ilanService = ilanService;
            _auditService = auditService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
        {
            const int pageSize = 10;
            var result = await _ilanService.GetPendingApprovalsAsync(page, pageSize, ct);

            var vm = new ModerasyonIndexViewModel();
            if (result.IsSuccess && result.Data != null)
            {
                vm.Listings = new EntityLayer.DTOs.Public.PagedResult<PendingListingRowViewModel>(
                    result.Data.Items.Select(x => new PendingListingRowViewModel
                    {
                        Id = x.Id,
                        Baslik = x.Baslik,
                        SeoSlug = x.SeoSlug,
                        Fiyat = x.Fiyat,
                        ParaBirimi = x.ParaBirimi,
                        Sehir = x.Sehir,
                        OlusturmaTarihi = x.OlusturmaTarihi,
                        KategoriAdi = x.KategoriAdi,
                        SahipAdSoyad = x.SahipAdSoyad,
                        SahipEmail = x.SahipEmail,
                        KapakFotoUrl = x.KapakFotoUrl
                    }).ToList(),
                    result.Data.TotalCount,
                    result.Data.Page,
                    result.Data.PageSize
                );
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["ErrorMessage"] = "Kullanıcı oturumu geçersiz.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _ilanService.ApproveAsync(id, userId, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "İlan onaylandı ve yayına alındı.";
                await _auditService.LogAsync("ListingApproved", "Ilan", id.ToString(), null, HttpContext.Connection.RemoteIpAddress?.ToString(), userId, ct);
            }
            else
                TempData["ErrorMessage"] = result.Error?.Message ?? "İlan onaylanırken bir hata oluştu.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(RejectListingViewModel model, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["ErrorMessage"] = "Kullanıcı oturumu geçersiz.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _ilanService.RejectAsync(model.ListingId, userId, model.RedNedeni, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "İlan reddedildi.";
                var detay = System.Text.Json.JsonSerializer.Serialize(new { RedNedeni = model.RedNedeni });
                await _auditService.LogAsync("ListingRejected", "Ilan", model.ListingId.ToString(), detay, HttpContext.Connection.RemoteIpAddress?.ToString(), userId, ct);
            }
            else
                TempData["ErrorMessage"] = result.Error?.Message ?? "İlan reddedilirken bir hata oluştu.";

            return RedirectToAction(nameof(Index));
        }
    }
}
