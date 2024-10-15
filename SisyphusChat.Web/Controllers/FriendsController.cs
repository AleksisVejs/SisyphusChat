using Microsoft.AspNetCore.Mvc;

namespace SisyphusChat.Web.Controllers
{
    public class FriendsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddFriends()
        {
            return View();
        }
    }
}
