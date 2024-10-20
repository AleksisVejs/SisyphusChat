using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;
using System.Security.Claims;
using SisyphusChat.Core.Services;

namespace SisyphusChat.Web.Controllers
{
    [ApiController]
    [Route("Notification")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public NotificationController(INotificationService notificationService, IUserService userService, UserManager<User> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _userService = userService;
        }

        [HttpGet("GetNotifications")]
        public async Task<IActionResult> GetNotifications()
        {
            try
            {
                var currentUser = await _userService.GetCurrentContextUserAsync();
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user ID

                // Fetch notifications using the user's ID
                var notifications = await _notificationService.GetUserNotificationsAsync(currentUser.Id);

                // Check if notifications are empty
                if (notifications == null || !notifications.Any())
                {
                    return Json(new
                    {
                        success = true,
                        message = "No notifications available.",
                        notifications = new List<NotificationModel>() // Return an empty list
                    });
                }

                // Return the notifications in a structured format
                return Json(new
                {
                    success = true,
                    message = "Notifications retrieved successfully.",
                    notifications
                });
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework like Serilog or NLog)
                System.Diagnostics.Debug.WriteLine($"Error fetching notifications: {ex.Message}");

                // Return a structured error response
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while fetching notifications."
                });
            }
        }

        [HttpPost("MarkAsRead")] // Specify the route for the post method
        public async Task<IActionResult> MarkAsRead(string notificationId)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(notificationId);
                return Ok();  // Return HTTP 200 OK if the operation succeeds
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);  // Return HTTP 404 if the notification was not found
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while marking the notification as read.");  // Return HTTP 500 for any other errors
            }
        }

    }
}
