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
            var currentUser = await _userService.GetCurrentContextUserAsync();

            if (currentUser == null)
            {
                _logger.LogWarning("❌ Current user is null");
                await Clients.Caller.SendAsync("ReceiveNotifications", Array.Empty<NotificationModel>());
                return;
            }

            var notifications = await _notificationService.GetNotificationsByUserId(currentUser.Id);
            
            // Ensure we always return an array, even if empty
            var notificationArray = notifications?.ToArray() ?? Array.Empty<NotificationModel>();
            
            _logger.LogInformation($"📫 Retrieved {notificationArray.Length} notifications");
            await Clients.Caller.SendAsync("ReceiveNotifications", notificationArray);
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

    public async Task MarkNotificationAsRead(string notificationId)
    {
        try
        {
            _logger.LogInformation($"Marking notification as read: {notificationId}");
            await _notificationService.MarkAsRead(notificationId);
            var currentUser = await _userService.GetCurrentContextUserAsync();
            await Clients.User(currentUser.Id).SendAsync("NotificationsUpdated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error marking notification as read");
            throw;
        }
    }

    public async Task MarkAllNotificationsAsRead()
    {
        try
        {
            _logger.LogInformation("Marking all notifications as read");
            var currentUser = await _userService.GetCurrentContextUserAsync();
            
            if (currentUser == null)
            {
                _logger.LogWarning("❌ Current user is null");
                return;
            }

            var notifications = await _notificationService.GetUnreadNotificationsByUserId(currentUser.Id);
            foreach (var notification in notifications)
            {
                await _notificationService.MarkAsRead(notification.Id);
            }

            await Clients.User(currentUser.Id).SendAsync("NotificationsUpdated");
            _logger.LogInformation("✅ Successfully marked all notifications as read");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error marking all notifications as read");
            throw;
        }
    }

    public async Task DeleteNotification(string notificationId)
    {
        try
        {
            _logger.LogInformation($"Deleting notification: {notificationId}");
            await _notificationService.DeleteByIdAsync(notificationId);
            var currentUser = await _userService.GetCurrentContextUserAsync();
            await Clients.User(currentUser.Id).SendAsync("NotificationsUpdated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error deleting notification");
            throw;
        }
    }

    
}
