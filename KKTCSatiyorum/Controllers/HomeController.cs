using System.Diagnostics;
using BusinessLayer.Features.Ilanlar.Services;
using BusinessLayer.Features.Kategoriler.Services;
using EntityLayer.DTOs.Public;
using KKTCSatiyorum.Models;
using KKTCSatiyorum.Models.Home;
using Microsoft.AspNetCore.Mvc;

namespace KKTCSatiyorum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IIlanService _ilanService;
        private readonly IKategoriService _kategoriService;

        public HomeController(ILogger<HomeController> logger, IIlanService ilanService, IKategoriService kategoriService)
        {
            _logger = logger;
            _ilanService = ilanService;
            _kategoriService = kategoriService;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var featuredResult = await _ilanService.SearchAsync(new ListingSearchQuery
            {
                Page = 1,
                PageSize = 12
            }, ct);

            var featuredListings = featuredResult.IsSuccess && featuredResult.Data != null
                ? featuredResult.Data.Items
                : Array.Empty<ListingCardDto>();

            var latestResult = await _ilanService.SearchAsync(new ListingSearchQuery
            {
                Page = 2,
                PageSize = 12
            }, ct);

            var latestListings = latestResult.IsSuccess && latestResult.Data != null
                ? latestResult.Data.Items
                : Array.Empty<ListingCardDto>();

            var categories = await BuildCategoryTreeAsync(ct);

            var viewModel = new HomeIndexViewModel
            {
                FeaturedListings = featuredListings,
                LatestListings = latestListings,
                Categories = categories
            };

            return View(viewModel);
        }

        private async Task<IReadOnlyList<CategoryNavItemViewModel>> BuildCategoryTreeAsync(CancellationToken ct)
        {
            try
            {
                var result = await _kategoriService.GetListAsync(ct);
                if (!result.IsSuccess || result.Data == null)
                    return Array.Empty<CategoryNavItemViewModel>();

                var activeCategories = result.Data.Where(c => c.AktifMi).ToList();

                var parents = activeCategories
                    .Where(c => c.UstKategoriId == null)
                    .OrderBy(c => c.SiraNo)
                    .ThenBy(c => c.Ad)
                    .Select(p => new CategoryNavItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Ad,
                        SiraNo = p.SiraNo,
                        Children = activeCategories
                            .Where(c => c.UstKategoriId == p.Id)
                            .OrderBy(c => c.SiraNo)
                            .ThenBy(c => c.Ad)
                            .Select(ch => new CategoryNavItemViewModel
                            {
                                Id = ch.Id,
                                Name = ch.Ad,
                                SiraNo = ch.SiraNo
                            })
                            .ToList()
                    })
                    .ToList();

                return parents;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to load categories for home page");
                return Array.Empty<CategoryNavItemViewModel>();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult HowItWorks()
        {
            return View();
        }

        public IActionResult Faq()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult SafeShopping()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult Kvkk()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public new IActionResult StatusCode(int code)
        {
            Response.StatusCode = code;
            if (code == 404)
                return View("NotFound");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
