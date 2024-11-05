// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using SisyphusChat.Web.Hubs;

namespace SisyphusChat.Web.Areas.Identity.Pages.Account.Manage
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<DeletePersonalDataModel> _logger;
        private readonly IUserDeletionService _userDeletionService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public DeletePersonalDataModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<DeletePersonalDataModel> logger,
            IUserDeletionService userDeletionService,
            IHubContext<NotificationHub> hubContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _userDeletionService = userDeletionService;
            _hubContext = hubContext;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool RequirePassword { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (user.IsAdmin)
            {
                ModelState.AddModelError(string.Empty, "Admin accounts cannot be deleted.");
                return Page();
            }

            RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return Page();
                }
            }

            try
            {
                string userId = user.Id;
                await _userDeletionService.DeleteUserAndRelatedDataAsync(userId);
                await _hubContext.Clients.All.SendAsync("UserDeleted", userId, $"DELETED_USER_{userId}");
                await _signInManager.SignOutAsync();
                _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);
                return Redirect("~/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", user.Id);
                ModelState.AddModelError(string.Empty, "An error occurred while deleting your account.");
                return Page();
            }
        }
    }
}
