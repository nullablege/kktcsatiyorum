using BusinessLayer.Features.Kategoriler.DTOs;
using BusinessLayer.Features.Kategoriler.Services;
using KKTCSatiyorum.Areas.Admin.Models;
using KKTCSatiyorum.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KKTCSatiyorum.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KategoriController : Controller
    {
        private readonly IKategoriService _kategoriService;
        public KategoriController(IKategoriService kategoriService)
        {
            _kategoriService = kategoriService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var result = await _kategoriService.GetListAsync(ct); 

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "Kategoriler yüklenemedi.";
                return View(Array.Empty<KategoriListItemDto>());
            }

            return View(result.Data ?? Array.Empty<KategoriListItemDto>());
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

        private async Task<IReadOnlyList<SelectListItem>> BuildUstKategoriOptionsAsync(CancellationToken ct)
        {
            var result = await _kategoriService.GetForDropdownAsync(ct);

            var items = new List<SelectListItem>
    {
        new SelectListItem { Value = "", Text = "-- Ana Kategori --" }
    };

            if (result.IsSuccess && result.Data != null)
            {
                items.AddRange(result.Data.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Ad
                }));
            }

            return items;
        }




    }
}
