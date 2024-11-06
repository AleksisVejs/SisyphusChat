using SisyphusChat.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Infrastructure.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        public Task<List<Notification>> GetNotificationsByUserId(string userId);
        public Task<List<Notification>> GetUnreadNotificationsByUserId(string userId);
        public Task MarkAsRead(string notificationId);
        public Task MarkAllAsReadAsync(string userId);
    }
}
