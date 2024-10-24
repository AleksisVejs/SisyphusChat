using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Web.Models;
using System.Diagnostics;
using SisyphusChat.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using SisyphusChat.Core.Services;
using SisyphusChat.Core.Interfaces;

namespace SisyphusChat.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, IUserService userService, UserManager<User> userManager)
        {
            _logger = logger;
            _userService = userService;
            _userManager = userManager;
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
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateOnlineStatus(bool isOnline)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                user.IsOnline = isOnline;
                await _userManager.UpdateAsync(user);
                return Ok();
            }
            return BadRequest();
        }

    }
}
