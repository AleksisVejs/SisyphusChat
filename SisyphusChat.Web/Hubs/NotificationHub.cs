using Microsoft.AspNetCore.SignalR;
using SisyphusChat.Core.Models;
using SisyphusChat.Core.Interfaces;

namespace SisyphusChat.Web.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly INotificationService notificationService;

        public NotificationHub(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public async Task NotifyNewMessage(string userId, string senderUsername)
        {
            // To Create the notification
            await notificationService.AddNotificationAsync(userId, "UnseenMessage", senderUsername);

            //To Notify the specific user about the new message
            await Clients.User(userId).SendAsync("ReceiveNewMessage", senderUsername);
        }

        public async Task NotifyUserMentioned(string userId, string mentionedBy)
        {
            await notificationService.AddNotificationAsync(userId, "Mentioned", mentionedBy);

            // To Notify the specific user about being mentioned
            await Clients.User(userId).SendAsync("ReceiveMention", mentionedBy);
        }
        public async Task NotifySystemUpdate(string userId, string mentionedBy)
        {
            await notificationService.AddNotificationAsync(userId, "SystemUpdate", mentionedBy);

            // To Notify the specific user about being mentioned
            await Clients.User(userId).SendAsync("SystemUpdate", mentionedBy);
        }
        public async Task NotifyNewFriendAdded(string userId, string friendUser)
        {
            // To Create the notification
            await notificationService.AddNotificationAsync(userId, "FriendRequest", friendUser);

            //To notify the specific user about the new friend added
            await Clients.User(userId).SendAsync("ReceiveNewFriend", friendUser);
        }

    }
}
