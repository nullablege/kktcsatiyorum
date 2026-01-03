using System.Diagnostics;
using BusinessLayer.Features.Ilanlar.Services;
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

        public HomeController(ILogger<HomeController> logger, IIlanService ilanService)
        {
            _logger = logger;
            _ilanService = ilanService;
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

            var viewModel = new HomeIndexViewModel
            {
                FeaturedListings = featuredListings,
                LatestListings = latestListings
            };

            return View(viewModel);
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
    }
}
