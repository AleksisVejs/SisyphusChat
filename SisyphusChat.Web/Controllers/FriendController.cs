using Microsoft.AspNetCore.Mvc;

namespace SisyphusChat.Web.Controllers
{
    public class FriendController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
