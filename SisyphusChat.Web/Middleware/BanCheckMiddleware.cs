using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Web.Models;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Web.Middleware
{
    public class BanCheckMiddleware
    {
        private readonly RequestDelegate _next;

        public BanCheckMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager<User> userManager)
        {
            if (context.Request.Path.StartsWithSegments("/Identity/Account/Logout") ||
                context.Request.Path.StartsWithSegments("/Home/Error")
              )
            {
                await _next(context);
                return;
            }
            // to check if user is authenticated
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user != null && user.IsBanned)
                {
                    // to check if user is not already on the banned page
                    if (!context.Request.Path.StartsWithSegments("/Home/Banned") && 
                        !context.Request.Path.StartsWithSegments("/Home/PermanentlyBanned"))
                    {
                        if (user.BanStart.HasValue && !user.BanEnd.HasValue) // Permanent ban
                        {
                            context.Response.Redirect("/Home/PermanentlyBanned", false);
                        }
                        else if (user.BanEnd.HasValue) // Temporary ban
                        {
                            var banEndEncoded = Uri.EscapeDataString(user.BanEnd.Value.ToString("o"));
                            context.Response.Redirect($"/Home/Banned?banEnd={banEndEncoded}", false);
                        }
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
} 