using BusinessLayer.Features.Ilanlar.DTOs;
using BusinessLayer.Features.Ilanlar.Services;
using BusinessLayer.Features.KategoriAlanlari.Services;
using BusinessLayer.Features.Kategoriler.Services;
using EntityLayer.Enums;
using KKTCSatiyorum.Areas.Member.Models;
using KKTCSatiyorum.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace KKTCSatiyorum.Areas.Member.Controllers
{
    public class MyListingsController : MemberBaseController
    {
        private readonly IIlanService _ilanService;
        private readonly IKategoriService _kategoriService;
        private readonly IKategoriAlaniService _kategoriAlaniService;
        private readonly IFileStorage _fileStorage;

        public MyListingsController(
            IIlanService ilanService,
            IKategoriService kategoriService,
            IKategoriAlaniService kategoriAlaniService,
            IFileStorage fileStorage)
        {
            _ilanService = ilanService;
            _kategoriService = kategoriService;
            _kategoriAlaniService = kategoriAlaniService;
            _fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            // TODO: İlanları listele
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct)
        {
            var model = new CreateIlanViewModel
            {
                KategoriOptions = await BuildKategoriOptionsAsync(ct),
                ParaBirimiOptions = BuildParaBirimiOptions()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateIlanViewModel model, List<IFormFile> photos, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var photoPaths = new List<string>();
            
            void CleanupPhotos()
            {
                foreach (var path in photoPaths)
                {
                    _fileStorage.Delete(path);
                }
            }

            try
            {
                foreach (var photo in photos)
                {
                    if (_fileStorage.IsValidImage(photo))
                    {
                        var path = await _fileStorage.SaveAsync(photo, "ilanlar", ct);
                        photoPaths.Add(path);
                    }
                }

                if (photoPaths.Count == 0)
                {
                    ModelState.AddModelError("photos", "En az bir geçerli fotoğraf yükleyiniz.");
                    return await ReturnViewWithData(model, ct);
                }

                var request = new CreateIlanRequest(
                    model.KategoriId,
                    model.Baslik,
                    model.Aciklama,
                    model.Fiyat,
                    model.ParaBirimi,
                    model.Sehir,
                    model.Attributes.Select(a => new AttributeValueInput(a.KategoriAlaniId, a.Value)).ToList(),
                    photoPaths
                );

                var result = await _ilanService.CreateAsync(request, userId, ct);

                if (result.IsSuccess)
                {
                    TempData["SuccessMessage"] = "İlanınız onaya gönderildi.";
                    return RedirectToAction(nameof(Index));
                }

                // Validation/service fail: fotoğrafları temizle
                CleanupPhotos();
                result.AddToModelState(ModelState);
            }
            catch
            {
                CleanupPhotos();
                throw;
            }

            return await ReturnViewWithData(model, ct);
        }

        private async Task<IActionResult> ReturnViewWithData(CreateIlanViewModel model, CancellationToken ct)
        {
            model.KategoriOptions = await BuildKategoriOptionsAsync(ct);
            model.ParaBirimiOptions = BuildParaBirimiOptions();

            if (model.KategoriId > 0)
            {
                var attrs = await _kategoriAlaniService.GetListByKategoriAsync(model.KategoriId, ct);
                if (attrs.IsSuccess)
                {
                    ViewBag.KategoriAlanlari = attrs.Data;
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetAttributesForCategory(int kategoriId, CancellationToken ct)
        {
            var result = await _kategoriAlaniService.GetListForFormAsync(kategoriId, ct);
            if (!result.IsSuccess)
            {
                return Json(new List<KategoriAttributesDto>());
            }

            var dtos = result.Data!.Select(a => new KategoriAttributesDto
            {
                Id = a.Id,
                Ad = a.Ad,
                Anahtar = a.Anahtar,
                VeriTipi = a.VeriTipi,
                ZorunluMu = a.ZorunluMu,
                Secenekler = a.Secenekler?.Select(s => new SecenekDto { Id = s.Id, Deger = s.Deger }).ToList() ?? new()
            }).ToList();

            return Json(dtos);
        }

        private async Task<IReadOnlyList<SelectListItem>> BuildKategoriOptionsAsync(CancellationToken ct)
        {
            var result = await _kategoriService.GetForDropdownAsync(ct);
            var items = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Kategori Seçin --" }
            };

            if (result.IsSuccess && result.Data != null)
            {
                items.AddRange(result.Data.Select(k => new SelectListItem
                {
                    Value = k.Id.ToString(),
                    Text = k.Ad
                }));
            }

            return items;
        }

        private static IReadOnlyList<SelectListItem> BuildParaBirimiOptions()
        {
            return Enum.GetValues<ParaBirimi>()
                .Select(p => new SelectListItem
                {
                    Value = ((int)p).ToString(),
                    Text = p.ToString()
                })
                .ToList();
        }
    }
}
