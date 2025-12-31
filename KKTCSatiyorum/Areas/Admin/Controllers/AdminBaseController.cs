using EntityLayer.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KKTCSatiyorum.Areas.Admin.Controllers
{
    [Area(RoleNames.Admin)]
    [Authorize(Roles = RoleNames.Admin)]
    public abstract class AdminBaseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
