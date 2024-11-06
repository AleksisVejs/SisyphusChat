using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Core.Models;

namespace SisyphusChat.Core.Interfaces
{
    public interface INotificationService : ICrud<NotificationModel>
    {
        Task<List<NotificationModel>> GetNotificationsByUserId(string userId);
        Task<List<NotificationModel>> GetUnreadNotificationsByUserId(string userId);
        Task MarkAsRead(string notificationId);
        Task MarkAllAsReadAsync(string userId);
    }
}
