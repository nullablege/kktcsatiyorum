using EntityLayer.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KKTCSatiyorum.Areas.Member.Controllers
{
    [Area("Member")]
    [Authorize(Roles = RoleNames.User)]
    public class MemberBaseController : Controller
    {
    }
}
