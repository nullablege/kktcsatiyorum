using BusinessLayer.Features.KategoriAlanlari.DTOs;
using BusinessLayer.Features.KategoriAlanlari.Services;
using BusinessLayer.Features.Kategoriler.Services;
using EntityLayer.Enums;
using KKTCSatiyorum.Areas.Admin.Models;
using KKTCSatiyorum.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KKTCSatiyorum.Areas.Admin.Controllers
{
    public class KategoriAlaniController : AdminBaseController
    {
        private readonly IKategoriAlaniService _kategoriAlaniService;
        private readonly IKategoriService _kategoriService;

        public KategoriAlaniController(
            IKategoriAlaniService kategoriAlaniService,
            IKategoriService kategoriService)
        {
            _kategoriAlaniService = kategoriAlaniService;
            _kategoriService = kategoriService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? kategoriId, CancellationToken ct)
        {
            var kategoriler = await _kategoriService.GetListAsync(ct);
            
            var vm = new KategoriAlaniIndexViewModel
            {
                SelectedKategoriId = kategoriId,
                KategoriOptions = BuildKategoriOptions(kategoriler.Data)
            };

            if (kategoriId.HasValue && kategoriId.Value > 0)
            {
                var kategori = await _kategoriService.GetByIdAsync(kategoriId.Value, ct);
                if (kategori.IsSuccess && kategori.Data != null)
                {
                    vm.KategoriAdi = kategori.Data.Ad;
                }

                var result = await _kategoriAlaniService.GetListByKategoriAsync(kategoriId.Value, ct);
                if (result.IsSuccess)
                {
                    vm.Items = result.Data ?? Array.Empty<KategoriAlaniListItemDto>();
                }
                else
                {
                    TempData["ErrorMessage"] = result.Error?.Message ?? "Alanlar yüklenemedi.";
                }
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int kategoriId, CancellationToken ct)
        {
            var kategori = await _kategoriService.GetByIdAsync(kategoriId, ct);
            if (!kategori.IsSuccess || kategori.Data == null)
            {
                TempData["ErrorMessage"] = "Kategori bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var model = new CreateKategoriAlaniViewModel
            {
                KategoriId = kategoriId,
                KategoriAdi = kategori.Data.Ad,
                VeriTipiOptions = BuildVeriTipiOptions()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateKategoriAlaniViewModel model, CancellationToken ct)
        {
            List<string>? secenekler = null;
            if (!string.IsNullOrWhiteSpace(model.SeceneklerText) && model.VeriTipi == VeriTipi.TekSecim)
            {
                secenekler = model.SeceneklerText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            }

            var request = new CreateKategoriAlaniRequest(
                model.KategoriId,
                model.Ad,
                model.Anahtar,
                model.VeriTipi,
                model.ZorunluMu,
                model.FiltrelenebilirMi,
                model.SiraNo,
                secenekler
            );

            var result = await _kategoriAlaniService.CreateAsync(request, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = $"Alan oluşturuldu. Id: {result.Data}";
                return RedirectToAction(nameof(Index), new { kategoriId = model.KategoriId });
            }

            result.AddToModelState(ModelState);
            model.VeriTipiOptions = BuildVeriTipiOptions();

            var kategori = await _kategoriService.GetByIdAsync(model.KategoriId, ct);
            model.KategoriAdi = kategori.Data?.Ad;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _kategoriAlaniService.GetByIdAsync(id, ct);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "Alan bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var data = result.Data;
            var model = new EditKategoriAlaniViewModel
            {
                Id = data.Id,
                KategoriId = data.KategoriId,
                KategoriAdi = data.KategoriAdi,
                Ad = data.Ad,
                Anahtar = data.Anahtar,
                VeriTipi = data.VeriTipi,
                ZorunluMu = data.ZorunluMu,
                FiltrelenebilirMi = data.FiltrelenebilirMi,
                SiraNo = data.SiraNo,
                AktifMi = data.AktifMi,
                VeriTipiOptions = BuildVeriTipiOptions(),
                Secenekler = data.Secenekler
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditKategoriAlaniViewModel model, CancellationToken ct)
        {
            var request = new UpdateKategoriAlaniRequest(
                model.Id,
                model.Ad,
                model.Anahtar,
                model.VeriTipi,
                model.ZorunluMu,
                model.FiltrelenebilirMi,
                model.SiraNo,
                model.AktifMi
            );

            var result = await _kategoriAlaniService.UpdateAsync(request, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Alan güncellendi.";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }

            result.AddToModelState(ModelState);
            model.VeriTipiOptions = BuildVeriTipiOptions();

            var detail = await _kategoriAlaniService.GetByIdAsync(model.Id, ct);
            model.Secenekler = detail.Data?.Secenekler ?? (IReadOnlyList<KategoriAlaniSecenegiDto>)Array.Empty<KategoriAlaniSecenegiDto>();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id, int kategoriId, CancellationToken ct)
        {
            var result = await _kategoriAlaniService.DeactivateAsync(id, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Alan pasife çekildi.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "İşlem başarısız.";
            }

            return RedirectToAction(nameof(Index), new { kategoriId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOption(int attributeId, string deger, CancellationToken ct)
        {
            var result = await _kategoriAlaniService.AddOptionAsync(attributeId, deger, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Seçenek eklendi.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "Seçenek eklenemedi.";
            }

            return RedirectToAction(nameof(Edit), new { id = attributeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateOption(int optionId, int attributeId, CancellationToken ct)
        {
            var result = await _kategoriAlaniService.DeactivateOptionAsync(optionId, ct);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Seçenek pasife çekildi.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "İşlem başarısız.";
            }

            return RedirectToAction(nameof(Edit), new { id = attributeId });
        }

        private static IReadOnlyList<SelectListItem> BuildKategoriOptions(IReadOnlyList<BusinessLayer.Features.Kategoriler.DTOs.KategoriListItemDto>? kategoriler)
        {
            var items = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- Kategori Seçin --" }
            };

            if (kategoriler != null)
            {
                items.AddRange(kategoriler.Select(k => new SelectListItem
                {
                    Value = k.Id.ToString(),
                    Text = k.Ad
                }));
            }

            return items;
        }

        private static IReadOnlyList<SelectListItem> BuildVeriTipiOptions()
        {
            return Enum.GetValues<VeriTipi>()
                .Select(v => new SelectListItem
                {
                    Value = ((int)v).ToString(),
                    Text = v switch
                    {
                        VeriTipi.Metin => "Metin",
                        VeriTipi.TamSayi => "Tam Sayı",
                        VeriTipi.Ondalik => "Ondalık",
                        VeriTipi.DogruYanlis => "Doğru/Yanlış",
                        VeriTipi.Tarih => "Tarih",
                        VeriTipi.TekSecim => "Tek Seçim",
                        _ => v.ToString()
                    }
                })
                .ToList();
        }
    }
}
