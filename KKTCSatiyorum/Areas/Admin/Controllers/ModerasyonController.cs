using Microsoft.AspNetCore.Mvc;

namespace KKTCSatiyorum.Areas.Admin.Controllers
{
    public class ModerasyonController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
