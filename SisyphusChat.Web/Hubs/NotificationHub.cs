using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;
using System;
using System.Threading.Tasks;

public class NotificationHub : Hub
{
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(
        IUserService userService,
        INotificationService notificationService,
        ILogger<NotificationHub> logger)
    {
        _userService = userService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("⭐ Client connecting to NotificationHub");
        try
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "Connected successfully!");
            await base.OnConnectedAsync();
            _logger.LogInformation("✅ Client connected successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error during connection");
            throw;
        }
    }

    public async Task GetNotificationsAsync()
    {
        _logger.LogInformation("⭐ Starting GetNotificationsAsync");
        try
        {
            _logger.LogInformation("📍 Attempting to get current user");
            var currentUser = await _userService.GetCurrentContextUserAsync();

            if (currentUser == null)
            {
                _logger.LogWarning("❌ Current user is null");
                await Clients.Caller.SendAsync("ReceiveNotifications", null);
                return;
            }

            _logger.LogInformation($"👤 Got current user with ID: {currentUser.Id}");

            _logger.LogInformation("📍 Fetching notifications for user");
            var notifications = await _notificationService.GetNotificationsByUserId(currentUser.Id);

            _logger.LogInformation($"📫 Retrieved {notifications?.Count() ?? 0} notifications");

            await Clients.Caller.SendAsync("ReceiveNotifications", notifications);
            _logger.LogInformation("✅ Successfully sent notifications to client");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in GetNotificationsAsync");
            await Clients.Caller.SendAsync("NotificationError", "Failed to fetch notifications");
        }
    }

    public async Task MarkNotificationAsRead(string notificationId)
    {
        _logger.LogInformation($"📍 Marking notification {notificationId} as read");
        try
        {
            await _notificationService.MarkAsRead(notificationId);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error marking notification as read");
            await Clients.Caller.SendAsync("NotificationError", "Failed to mark notification as read");
        }
    }

    public async Task SendNotificationAsync(string userId, string message)
    {
        _logger.LogInformation($"📍 Sending notification to user {userId}");
        try
        {
            var notification = new NotificationModel
            {
                UserId = userId,
                Message = message,
                TimeCreated = DateTime.UtcNow,
                IsRead = false
            };

            await _notificationService.CreateAsync(notification);

            _logger.LogInformation("✅ Notification added to database");
            await Clients.User(userId).SendAsync("ReceiveNotification", notification);
            _logger.LogInformation("✅ Notification sent to client");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error sending notification");
            await Clients.Caller.SendAsync("NotificationError", "Failed to send notification");
        }
    }
}
