using Microsoft.AspNetCore.Mvc;

namespace KKTCSatiyorum.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
