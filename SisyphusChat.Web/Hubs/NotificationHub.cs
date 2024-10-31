using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;

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

 

    public async Task MarkMessageNotificationsAsRead(string chatId, string userId)
    {
        try
        {
            var notifications = await _notificationService.GetUnreadNotificationsByUserId(userId);
            var messageNotifications = notifications.Where(n => n.Type == NotificationType.Message && n.RelatedEntityId == chatId);
            
            foreach (var notification in messageNotifications)
            {
                await _notificationService.MarkAsRead(notification.Id.ToString());
            }
            
            await Clients.User(userId).SendAsync("NotificationsUpdated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error marking message notifications as read");
        }
    }

    public async Task MarkSingleNotificationAsRead(string notificationId)
    {
        try
        {
            _logger.LogInformation($"Marking single notification as read: {notificationId}");
            await _notificationService.MarkAsRead(notificationId);
            var currentUser = await _userService.GetCurrentContextUserAsync();
            await Clients.User(currentUser.Id).SendAsync("NotificationsUpdated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error marking single notification as read");
            throw;
        }
    }

    
}
