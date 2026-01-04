using BusinessLayer.Features.Ilanlar.DTOs;
using BusinessLayer.Features.Ilanlar.Services;
using BusinessLayer.Features.KategoriAlanlari.Services;
using BusinessLayer.Features.Kategoriler.Services;
using EntityLayer.Enums;
using KKTCSatiyorum.Areas.Member.Models.MyListings;
using KKTCSatiyorum.Models.Common;
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
        public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            const int pageSize = 10;
            var result = await _ilanService.GetMyListingsAsync(userId, page, pageSize, ct);

            var vm = new MyListingsIndexViewModel();

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "İlanlar yüklenirken bir hata oluştu.";
                return View(vm);
            }

            if (result.Data != null)
            {
                vm.Listings = new PagedViewModel<MyListingRowViewModel>
                {
                    Items = result.Data.Items.Select(x => new MyListingRowViewModel
                    {
                        Id = x.Id,
                        Baslik = x.Baslik,
                        SeoSlug = x.SeoSlug,
                        Fiyat = x.Fiyat,
                        ParaBirimi = x.ParaBirimi,
                        Durum = x.Durum,
                        RedNedeni = x.RedNedeni,
                        OlusturmaTarihi = x.OlusturmaTarihi,
                        YayinTarihi = x.YayinTarihi,
                        KategoriAdi = x.KategoriAdi,
                        KapakFotoUrl = x.KapakFotoUrl
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
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _ilanService.DeleteMyListingAsync(id, userId, ct);

            if (result.IsSuccess)
                TempData["SuccessMessage"] = "İlan silindi.";
            else
                TempData["ErrorMessage"] = result.Error?.Message ?? "İlan silinirken bir hata oluştu.";

            return RedirectToAction(nameof(Index));
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
                    model.Ilce,
                    model.Enlem,
                    model.Boylam,
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _ilanService.GetMyListingForEditAsync(id, userId, ct);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "İlan bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var dto = result.Data!;
            var model = new EditIlanViewModel
            {
                Id = dto.Id,
                KategoriId = dto.KategoriId,
                Baslik = dto.Baslik,
                Aciklama = dto.Aciklama,
                Fiyat = dto.Fiyat,
                ParaBirimi = dto.ParaBirimi,
                Sehir = dto.Sehir ?? "",
                Ilce = dto.Ilce ?? "",
                Enlem = dto.Enlem,
                Boylam = dto.Boylam,
                CurrentPhotos = dto.Photos,
                // Map existing attributes to input model
                Attributes = dto.Attributes.Select(a => new AttributeInputModel
                {
                    KategoriAlaniId = a.KategoriAlaniId,
                    Value = a.Value ?? ""
                }).ToList(),
                
                KategoriOptions = await BuildKategoriOptionsAsync(ct),
                ParaBirimiOptions = BuildParaBirimiOptions()
            };

            // We also need to pass the category attributes definition to View similarly to Create
            // so the form can render the inputs correctly (labels, types, options).
            // We can reuse 'GetAttributesForCategory' logic or just pass what we have if the View can handle it.
            // But Create view uses JS to fetch attributes on category change. 
            // For Edit, we ideally preload them. 
            // The DTO now has some metadata (Ad, VeriTipi, Secenekler), but the View (Create.cshtml logic) 
            // might expect the JSON structure from `GetAttributesForCategory`.
            // Let's populate ViewBag.KategoriAlanlari as in ReturnViewWithData.
            
            var attrsResult = await _kategoriAlaniService.GetListForFormAsync(dto.KategoriId, ct);
            if (attrsResult.IsSuccess)
            {
                 // We need to map this to what the view expects.
                 // Create view uses ViewBag.KategoriAlanlari which is List<KategoriAttributeDto> (from service).
                 ViewBag.KategoriAlanlari = attrsResult.Data;
            }

            return View(model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditIlanViewModel model, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (id != model.Id) return BadRequest();

            // Note: We don't handle photo uploads in this edit step yet as per requirements.
            // But if we did, we would process them here.

            if (!ModelState.IsValid)
            {
                // Re-populate dropdowns/attributes
                model.KategoriOptions = await BuildKategoriOptionsAsync(ct);
                model.ParaBirimiOptions = BuildParaBirimiOptions();
                if (model.KategoriId > 0)
                {
                    var attrs = await _kategoriAlaniService.GetListForFormAsync(model.KategoriId, ct);
                    if (attrs.IsSuccess) ViewBag.KategoriAlanlari = attrs.Data;
                }
                // Need to re-fetch current photos?
                // The service GetForEdit is needed again OR we trust the view to describe photos?
                // Usually we re-fetch to show them again.
                var existing = await _ilanService.GetMyListingForEditAsync(id, userId, ct);
                if (existing.IsSuccess)
                {
                    model.CurrentPhotos = existing.Data!.Photos;
                }

                return View(model);
            }

            var request = new UpdateIlanRequest(
                model.KategoriId,
                model.Baslik,
                model.Aciklama,
                model.Fiyat,
                model.ParaBirimi,
                model.Sehir,
                model.Ilce,
                model.Enlem,
                model.Boylam,
                model.Attributes.Select(a => new AttributeValueInput(a.KategoriAlaniId, a.Value)).ToList()
            );

            var result = await _ilanService.UpdateMyListingAsync(id, request, userId, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "İlan güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            // Error case
            TempData["ErrorMessage"] = result.Error?.Message ?? "Güncelleme sırasında bir hata oluştu.";
            
            // Re-populate for view
            model.KategoriOptions = await BuildKategoriOptionsAsync(ct);
            model.ParaBirimiOptions = BuildParaBirimiOptions();
             if (model.KategoriId > 0)
            {
                var attrs = await _kategoriAlaniService.GetListForFormAsync(model.KategoriId, ct);
                if (attrs.IsSuccess) ViewBag.KategoriAlanlari = attrs.Data;
            }
             var existingRel = await _ilanService.GetMyListingForEditAsync(id, userId, ct);
            if (existingRel.IsSuccess)
            {
                model.CurrentPhotos = existingRel.Data!.Photos;
            }

            return View(model);
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
