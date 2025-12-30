using BusinessLayer.Features.Favoriler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KKTCSatiyorum.Controllers
{
    [Authorize]
    public class FavoritesController : Controller
    {
        private readonly IFavoriService _favoriService;

        public FavoritesController(IFavoriService favoriService)
        {
            _favoriService = favoriService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int ilanId, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await _favoriService.ToggleAsync(ilanId, userId, ct);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Error?.Message ?? "İşlem başarısız." });
            }

            return Ok(new { isFavorite = result.Data!.IsFavorite });
        }
    }
}
