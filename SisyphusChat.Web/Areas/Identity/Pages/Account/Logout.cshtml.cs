using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Web.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager; // Add UserManager to handle user retrieval
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<User> signInManager, UserManager<User> userManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager; // Initialize UserManager
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // Retrieve the current user
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                // Update the IsOnline property
                user.IsOnline = false;
                await _userManager.UpdateAsync(user); // Save changes to the database
            }

            // Sign out the user
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            // Redirect to the specified return URL or the current page
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage("/Account/Login");
            }
        }
    }
}
