using BusinessLayer.Features.Favoriler.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KKTCSatiyorum.Controllers
{
    [Authorize(Roles = EntityLayer.Constants.RoleNames.User)]
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
                var error = result.Error;
                return error?.Type switch
                {
                    BusinessLayer.Common.Results.ErrorType.Validation => BadRequest(new { message = error.Message }),
                    BusinessLayer.Common.Results.ErrorType.NotFound => NotFound(new { message = error.Message }),
                    BusinessLayer.Common.Results.ErrorType.Conflict => Conflict(new { message = error.Message }),
                    _ => StatusCode(500, new { message = error?.Message ?? "İşlem başarısız." })
                };
            }

            return Ok(new { isFavorite = result.Data!.IsFavorite });
        }
    }
}
