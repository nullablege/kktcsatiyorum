using BusinessLayer.Features.Favoriler.Services;
using KKTCSatiyorum.Areas.Member.Models.Favorites;
using KKTCSatiyorum.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;

namespace KKTCSatiyorum.Areas.Member.Controllers
{
    public class FavoritesController : MemberBaseController
    {
        private readonly IFavoriService _favoriService;

        public FavoritesController(IFavoriService favoriService)
        {
            _favoriService = favoriService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, CancellationToken ct = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            const int pageSize = 12;
            var result = await _favoriService.GetMyFavoritesAsync(userId, page, pageSize, ct);

            var vm = new FavoritesIndexViewModel();

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "Favoriler yüklenirken bir hata oluştu.";
                return View(vm);
            }

            if (result.Data != null)
            {
                vm.Listings = new PagedViewModel<FavoriteListingRowViewModel>
                {
                    Items = result.Data.Items.Select(x => new FavoriteListingRowViewModel
                    {
                        IlanId = x.IlanId,
                        Baslik = x.Baslik,
                        SeoSlug = x.SeoSlug,
                        Fiyat = x.Fiyat,
                        Sehir = x.Sehir,
                        OlusturmaTarihi = x.OlusturmaTarihi,
                        KapakFotoUrl = x.KapakFotoUrl,
                        KategoriAdi = x.KategoriAdi
                    }).ToList(),
                    TotalCount = result.Data.TotalCount,
                    Page = result.Data.Page,
                    PageSize = result.Data.PageSize
                };
            }

            return View(vm);
        }
    }
}
