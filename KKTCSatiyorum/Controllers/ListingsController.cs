using BusinessLayer.Features.Ilanlar.Services;
using BusinessLayer.Features.KategoriAlanlari.Services;
using BusinessLayer.Features.Kategoriler.DTOs;
using BusinessLayer.Features.Kategoriler.Services;
using EntityLayer.DTOs.Public;
using KKTCSatiyorum.Models.Listings;
using Microsoft.AspNetCore.Mvc;

namespace KKTCSatiyorum.Controllers
{
    [Route("ilanlar")]
    public class ListingsController : Controller
    {
        private readonly IIlanService _ilanService;
        private readonly IKategoriService _kategoriService;
        private readonly IKategoriAlaniService _kategoriAlaniService;

        public ListingsController(
            IIlanService ilanService,
            IKategoriService kategoriService,
            IKategoriAlaniService kategoriAlaniService)
        {
            _ilanService = ilanService;
            _kategoriService = kategoriService;
            _kategoriAlaniService = kategoriAlaniService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] ListingSearchQuery query, CancellationToken ct)
        {
            var searchResult = await _ilanService.SearchAsync(query, ct);
            if (!searchResult.IsSuccess)
            {
                return View(new ListingsIndexViewModel
                {
                    Query = query,
                    Listings = new PagedResult<ListingCardDto>(
                        new List<ListingCardDto>(), 0, 1, 12)
                });
            }

            var kategoriler = await _kategoriService.GetForDropdownAsync(ct);

            var viewModel = new ListingsIndexViewModel
            {
                Query = query,
                Listings = searchResult.Data!,
                Kategoriler = kategoriler.Data ?? new List<KategoriDropdownItemDto>()
            };

            return View(viewModel);
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> Detail(string slug, CancellationToken ct)
        {
            var result = await _ilanService.GetPublicDetailBySlugAsync(slug, ct);
            if (!result.IsSuccess)
            {
                return NotFound();
            }

            return View(result.Data);
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilters(int kategoriId, CancellationToken ct)
        {
            var result = await _kategoriAlaniService.GetListForFormAsync(kategoriId, ct);
            if (!result.IsSuccess)
            {
                return Json(new List<object>());
            }

            // Return only filterable attributes
            var filterableAttrs = result.Data!
                .Where(a => a.FiltrelenebilirMi)
                .Select(a => new
                {
                    a.Id,
                    a.Ad,
                    a.VeriTipi,
                    Secenekler = a.Secenekler?.Select(s => new { s.Id, s.Deger }).ToList()
                })
                .ToList();

            return Json(filterableAttrs);
        }
    }
}


