using BusinessLayer.Features.Kategoriler.DTOs;
using BusinessLayer.Features.Kategoriler.Services;
using KKTCSatiyorum.Areas.Admin.Models;
using KKTCSatiyorum.Extensions;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateKategoriViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateKategoriViewModel model, CancellationToken ct)
        {
            var req = new CreateKategoriRequest(
                model.Ad ?? "",
                model.SeoSlug ?? "",
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
            return View(model);


        }
    }
}
