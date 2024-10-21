using Microsoft.AspNetCore.SignalR;
using SisyphusChat.Core.Models;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Web.Hubs
{
    public class NotificationHub(INotificationService notificationService) : Hub
    {

        public async Task NotifyNewMessage(string userId, string senderUsername, string chatid)
        {
            await notificationService.AddNotificationAsync(userId, "UnseenMessage", senderUsername, chatid);
            await Clients.User(userId).SendAsync("ReceiveNotification", $"New message from {senderUsername}");
        }
        /*
        public async Task NotifyUserMentioned(string userId, string mentionedBy)
        {
            await notificationService.AddNotificationAsync(userId, "Mentioned", mentionedBy);
            await Clients.User(userId).SendAsync("ReceiveNotification", $"{mentionedBy} mentioned you");
        }*/
        /*
        public async Task NotifyNewFriendAdded(string userId, string friendUser)
        {
            await notificationService.AddNotificationAsync(userId, "FriendRequest", friendUser);
            await Clients.User(userId).SendAsync("ReceiveNotification", $"Friend request from {friendUser}");
        }*/
        /*
        public async Task NotifySystemUpdate(string userId, string updateMessage)
        {
            await notificationService.AddNotificationAsync(userId, "SystemUpdate", updateMessage);
            await Clients.User(userId).SendAsync("ReceiveNotification", $"System Update: {updateMessage}");
        }
        */
        public async Task MarkAsRead(string notificationId)
        {
            await notificationService.MarkAsReadAsync(notificationId);
        }
    }
}
