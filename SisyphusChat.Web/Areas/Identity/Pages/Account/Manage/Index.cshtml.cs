﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Web.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Username { get; set; }
        public string Email { get; set; }


        [TempData]
        public string StatusMessage { get; set; }
        public string Avatar { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "New Username")]
            [StringLength(32, MinimumLength = 6, ErrorMessage = "Username should be between 6 and 32 characters.")]
            [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Username can only contain letters, digits, hyphens, underscores, and periods.")]
            public string NewUsername { get; set; }

            public byte[] Picture { get; set; }
            public IFormFile ProfilePicture { get; set; }
            public string CroppedImage { get; set; }

            [Display(Name = "Profanity Filter")]
            public bool IsProfanityEnabled { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            Username = userName;
            Input = new InputModel {
                NewUsername = "",
                Picture = user.Picture ?? GetDefaultAvatar(),
                IsProfanityEnabled = user.IsProfanityEnabled
            };
        }
        
        private byte[] GetDefaultAvatar()
        {
            return System.IO.File.ReadAllBytes("wwwroot/images/default_pfp.jpg");
        }
        
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Check if NewUsername is provided and validate uniqueness
            if (!string.IsNullOrWhiteSpace(Input.NewUsername))
            {
                var taken = await _userManager.FindByNameAsync(Input.NewUsername);
                if (taken != null && taken.Id != user.Id) // Ensure it's not the same user
                {
                    ModelState.AddModelError(string.Empty, $"Username '{Input.NewUsername}' is already taken.");
                    await LoadAsync(user); // Reload the user data to ensure the profile picture is loaded
                    return Page();
                }

                // Update the username if it's unique
                user.UserName = Input.NewUsername;
            }

            // Process the cropped image if provided
            if (!string.IsNullOrWhiteSpace(Input.CroppedImage))
            {
                byte[] imageBytes = Convert.FromBase64String(Input.CroppedImage);
                user.Picture = imageBytes;
            }
            else if (Input.ProfilePicture != null && Input.ProfilePicture.Length > 0)
            {
                if (Input.ProfilePicture.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError(string.Empty, "The profile picture size cannot exceed 2MB.");
                    await LoadAsync(user); // Reload the user data to ensure the profile picture is loaded
                    return Page();
                }
                using (var memoryStream = new MemoryStream())
                {
                    await Input.ProfilePicture.CopyToAsync(memoryStream);
                    user.Picture = memoryStream.ToArray();
                }
            }

            user.IsProfanityEnabled = Input.IsProfanityEnabled;

            if (!ModelState.IsValid)
            {
                await LoadAsync(user); // Reload the user data to ensure the profile picture is loaded
                return Page();
            }

            var result = await _userManager.UpdateAsync(user);
            user.LastUpdated = DateTime.Now;

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                await LoadAsync(user); // Reload the user data to ensure the profile picture is loaded
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}