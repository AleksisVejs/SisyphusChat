using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Attributes
{
    public class AdminOnlyAttribute : TypeFilterAttribute
    {
        public AdminOnlyAttribute() : base(typeof(AdminOnlyFilter))
        {
        }

        private class AdminOnlyFilter : IAuthorizationFilter
        {
            private readonly UserManager<User> _userManager;

            public AdminOnlyFilter(UserManager<User> userManager)
            {
                _userManager = userManager;
            }

            public async void OnAuthorization(AuthorizationFilterContext context)
            {
                var user = await _userManager.GetUserAsync(context.HttpContext.User);
                if (user == null || !user.IsAdmin)
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
                }
            }
        }
    }
} 