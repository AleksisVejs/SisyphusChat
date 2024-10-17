using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Core.Models;

namespace SisyphusChat.Core.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationModel>> GetUserNotificationsAsync(string userId);
        Task AddNotificationAsync(string userId, string notificationType, string senderUsername);
        Task MarkAsReadAsync(string notificationId);
        Task ClearNotificationsAsync(string notificationId);
    }

}
