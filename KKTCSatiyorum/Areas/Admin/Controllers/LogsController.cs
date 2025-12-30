using BusinessLayer.Features.DenetimKayitlari.DTOs;
using BusinessLayer.Features.DenetimKayitlari.Services;

using KKTCSatiyorum.Areas.Admin.Models.Logs;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace KKTCSatiyorum.Areas.Admin.Controllers
{
    public class LogsController : AdminBaseController
    {
        private readonly IDenetimKaydiService _service;

        public LogsController(IDenetimKaydiService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DenetimKaydiQuery query, CancellationToken ct)
        {
            if (query.Page <= 0) query.Page = 1;
            if (query.PageSize <= 0) query.PageSize = 25;

            var result = await _service.GetPagedAsync(query, ct);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.Error?.Message ?? "Loglar yüklenirken hata oluştu.";
                return View(new LogsIndexViewModel 
                { 
                    Results = new EntityLayer.DTOs.Public.PagedResult<DenetimKaydiListItemDto>(new System.Collections.Generic.List<DenetimKaydiListItemDto>(), 0, 1, 25),
                    Filter = query
                });
            }

            var model = new LogsIndexViewModel
            {
                Results = result.Data!,
                Filter = query
            };

            return View(model);
        }
    }
}
