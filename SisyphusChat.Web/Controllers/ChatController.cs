using Microsoft.AspNetCore.Mvc;

namespace SisyphusChat.Web.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
