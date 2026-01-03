using Microsoft.AspNetCore.Mvc;

namespace KKTCSatiyorum.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        public new IActionResult Index()
        {
            return View();
        }
    }
}
