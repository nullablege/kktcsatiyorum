using BusinessLayer.Features.Kategoriler.DTOs;
using BusinessLayer.Features.Kategoriler.Services;
using KKTCSatiyorum.Areas.Admin.Models;
using KKTCSatiyorum.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using EntityLayer.Constants;
namespace KKTCSatiyorum.Areas.Admin.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    [Area("Admin")]
    public class KategoriController : Controller
    {
        private readonly IKategoriService _kategoriService;
        public KategoriController(IKategoriService kategoriService)
        {
            _kategoriService = kategoriService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? id, CancellationToken ct)
        {
            var result = await _kategoriService.GetListAsync(ct); 

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "Kategoriler yüklenemedi.";
                return View(new KategoriIndexViewModel {Items = Array.Empty<KategoriListItemDto>()});
            }

            var vm = new KategoriIndexViewModel
            {
                Items = result.Data ?? Array.Empty<KategoriListItemDto>()
            };

            if (id.HasValue)
            {
                var detail = await _kategoriService.GetByIdAsync(id.Value, ct);
                if (detail.IsSuccess && detail.Data != null)
                {
                    vm.Selected = new UpdateKategoriViewModel
                    {
                        Id = detail.Data.Id,
                        Ad = detail.Data.Ad,
                        UstKategoriId = detail.Data.UstKategoriId,
                        SiraNo = detail.Data.SiraNo,
                        AktifMi = detail.Data.AktifMi,
                        UstKategoriOptions = await BuildUstKategoriOptionsAsync(ct, excludeId: detail.Data.Id)
                    };
                }
                else
                {
                    TempData["ErrorMessage"] = detail.Error?.Message ?? "Kategori bulunamadı.";
                }
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var model = new CreateKategoriViewModel
            {
                UstKategoriOptions = await BuildUstKategoriOptionsAsync(ct)
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateKategoriViewModel model, CancellationToken ct)
        {
            var req = new CreateKategoriRequest(
                model.Ad.Trim(),
                model.UstKategoriId,
                model.SiraNo
                );
            
            var result = await _kategoriService.CreateAsync(req, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Kategori oluşturuldu. Id: {result.Data!.Id}";
                return RedirectToAction(nameof(Index));
            }

                result.AddToModelState(ModelState);
            model.UstKategoriOptions = await BuildUstKategoriOptionsAsync(ct);
            return View(model);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([Bind(Prefix = "Selected")] UpdateKategoriViewModel model, CancellationToken ct)
        {
            var req = new UpdateKategoriRequest
            {
                Id = model.Id,
                Ad = model.Ad.Trim(),
                UstKategoriId = model.UstKategoriId,
                SiraNo = model.SiraNo,
                AktifMi = model.AktifMi
            };

            var result = await _kategoriService.UpdateAsync(req, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Kategori güncellendi.";
                return RedirectToAction(nameof(Index), new { id = model.Id });
            }

            result.AddToModelState(ModelState, "Selected");

            var categories = await _kategoriService.GetListAsync(ct);

            var vm = new KategoriIndexViewModel
            {
                Items = categories.IsSuccess ? (categories.Data ?? Array.Empty<KategoriListItemDto>()) : Array.Empty<KategoriListItemDto>(),
                Selected = model
            };

            vm.Selected.UstKategoriOptions = await BuildUstKategoriOptionsAsync(ct, excludeId: model.Id);

            return View("Index", vm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id, CancellationToken ct)
        {
            var result = await _kategoriService.SoftDeleteAsync(new SoftDeleteKategoriRequest { Id = id }, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Kategori pasife çekildi.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = result.Error?.Message ?? "Silme işlemi başarısız.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IReadOnlyList<SelectListItem>> BuildUstKategoriOptionsAsync(CancellationToken ct, int? excludeId = null)
        {
            var result = await _kategoriService.GetForDropdownAsync(ct);

            var items = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "-- Ana Kategori --" }
    };

            if (result.IsSuccess && result.Data != null)
            {
                var filtered = excludeId.HasValue
                ? result.Data.Where(x => x.Id != excludeId.Value)
                : result.Data;

                items.AddRange(filtered.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Ad
                }));
            }

            return items;
        }




    }
}
