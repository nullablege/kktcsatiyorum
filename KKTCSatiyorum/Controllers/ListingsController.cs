using BusinessLayer.Features.Ilanlar.Services;
using BusinessLayer.Features.KategoriAlanlari.Services;
using BusinessLayer.Features.Kategoriler.Services;
using BusinessLayer.Features.Favoriler.Services;
using EntityLayer.DTOs.Public;
using KKTCSatiyorum.Models.Listings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace KKTCSatiyorum.Controllers
{
    [Route("ilanlar")]
    public class ListingsController : Controller
    {
        private readonly IIlanService _ilanService;
        private readonly IKategoriService _kategoriService;
        private readonly IKategoriAlaniService _kategoriAlaniService;

        private readonly IFavoriService _favoriService;

        // Static sort options - no need to recreate every request
        private static readonly IEnumerable<SelectListItem> _sortOptions = new List<SelectListItem>
        {
            new SelectListItem("Tarih: Yeniden Eskiye", ""),
            new SelectListItem("Tarih: Eskiden Yeniye", "eski"),
            new SelectListItem("Fiyat: Artan", "fiyat_artan"),
            new SelectListItem("Fiyat: Azalan", "fiyat_azalan")
        };

        public ListingsController(
            IIlanService ilanService,
            IKategoriService kategoriService,
            IKategoriAlaniService kategoriAlaniService,
            IFavoriService favoriService)
        {
            _ilanService = ilanService;
            _kategoriService = kategoriService;
            _kategoriAlaniService = kategoriAlaniService;
            _favoriService = favoriService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] ListingSearchQuery query, CancellationToken ct)
        {
            var kategoriler = await _kategoriService.GetForDropdownAsync(ct);
            var kategoriOptions = (kategoriler.Data ?? Enumerable.Empty<BusinessLayer.Features.Kategoriler.DTOs.KategoriDropdownItemDto>())
                .Select(k => new SelectListItem(k.Ad, k.Id.ToString()));

            var searchResult = await _ilanService.SearchAsync(query, ct);
            if (!searchResult.IsSuccess)
            {
                return View(new ListingsIndexViewModel
                {
                    Query = query,
                    Listings = new PagedResult<ListingCardDto>(
                        new List<ListingCardDto>(), 0, 1, 12),
                    KategoriOptions = kategoriOptions,
                    SortOptions = _sortOptions
                });
            }

            var viewModel = new ListingsIndexViewModel
            {
                Query = query,
                Listings = searchResult.Data!,
                KategoriOptions = kategoriOptions,
                SortOptions = _sortOptions
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

            bool isFavorite = false;
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = System.Security.Claims.ClaimsPrincipal.Current?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                             ?? User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
                
                if (!string.IsNullOrEmpty(userId))
                {
                     var favResult = await _favoriService.IsFavoriteAsync(result.Data.Id, userId, ct);
                     isFavorite = favResult.IsSuccess && favResult.Data;
                }
            }
            ViewBag.IsFavorite = isFavorite;

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



