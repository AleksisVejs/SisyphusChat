using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Web.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Banned(string banEnd)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsBanned)
            {
                return RedirectToAction("Index");
            }

            if (!DateTime.TryParse(banEnd, out DateTime banEndDate))
            {
                _logger.LogError("Invalid ban end date format: {BanEnd}", banEnd);
                return RedirectToAction("Error");
            }

            var viewModel = new BannedViewModel
            {
                BanEnd = banEndDate,
                ReturnUrl = Request.Headers["Referer"].ToString() ?? "/"
            };

            return View(viewModel);
        }

        public IActionResult PermanentlyBanned()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            return View();
        }
    }
}
