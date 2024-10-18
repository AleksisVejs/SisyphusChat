using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Web.Hubs;
using System.Threading.Tasks;

namespace SisyphusChat.Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(INotificationService notificationService, IHubContext<NotificationHub> hubContext)
        {
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        // Action to handle a new message notification
        [HttpPost]
        public async Task<IActionResult> NotifyNewMessage(string userId, string senderUsername)
        {
            // Use the service to create the notification
            await _notificationService.AddNotificationAsync(userId, "UnseenMessage", senderUsername);

            // Notify the user via SignalR in real-time
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNewMessage", senderUsername);

            return Ok(new { success = true });
        }

        // Action to handle a user mention notification
        [HttpPost]
        public async Task<IActionResult> NotifyUserMentioned(string userId, string mentionedBy)
        {
            // Use the service to create the notification
            await _notificationService.AddNotificationAsync(userId, "Mentioned", mentionedBy);

            // Notify the user via SignalR in real-time
            await _hubContext.Clients.User(userId).SendAsync("ReceiveMention", mentionedBy);

            return Ok(new { success = true });
        }

        // Action to handle a system update notification
        [HttpPost]
        public async Task<IActionResult> NotifySystemUpdate(string userId, string systemUpdateInfo)
        {
            // Use the service to create the notification
            await _notificationService.AddNotificationAsync(userId, "SystemUpdate", systemUpdateInfo);

            // Notify the user via SignalR in real-time
            await _hubContext.Clients.User(userId).SendAsync("SystemUpdate", systemUpdateInfo);

            return Ok(new { success = true });
        }

        // Action to handle a new friend request notification
        [HttpPost]
        public async Task<IActionResult> NotifyNewFriendAdded(string userId, string friendUsername)
        {
            // Use the service to create the notification
            await _notificationService.AddNotificationAsync(userId, "FriendRequest", friendUsername);

            // Notify the user via SignalR in real-time
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNewFriend", friendUsername);

            return Ok(new { success = true });
        }

        // Action to retrieve all notifications for a user
        [HttpGet]
        public async Task<IActionResult> GetUserNotifications(string userId)
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(notifications);
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveNotificationVergGood(string notificationId)
        {
            // Call the service to remove the notification
            await _notificationService.ClearNotificationsAsync(notificationId);

            return Ok(new { success = true });
        }
    }
}
