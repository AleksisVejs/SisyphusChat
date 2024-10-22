using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Interfaces;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace SisyphusChat.Infrastructure.Repositories
{
    public class NotificationRepository(ApplicationDbContext context) : INotificationRepository
    {
        public async Task AddAsync(Notification notification)
        {
            notification.Id = Guid.NewGuid().ToString();

            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();
        }

        public async Task<ICollection<Notification>> GetAllAsync()
        {
            return await context.Notifications.ToListAsync();
        }

        public async Task<Notification> GetByIdAsync(string id)
        {
            var message = await context.Notifications.FirstOrDefaultAsync(g => g.Id.ToString() == id);
            return message;
        }
        public async Task<ICollection<Notification>> GetUserNotificationsAsync(string userId)
        {
            return await context.Notifications
                .Where(n => n.UserId == userId) // Filter notifications for the specified user
                .ToListAsync();
        }

        public async Task DeleteByIdAsync(string id)
        {
            var notification = await GetByIdAsync(id);
            if (notification != null)
            {
                context.Notifications.Remove(notification);
                await context.SaveChangesAsync();
            }
        }
        public async Task DeleteByUsernameAsync(string userId, string senderUsername)
        {
            // Fetch all notifications for the specified userId and senderUsername
            var notifications = await context.Notifications
                .Where(n => n.UserId == userId && n.SenderUsername == senderUsername) // Adjust this to your condition
                .ToListAsync();

            if (notifications != null && notifications.Any())
            {
                context.Notifications.RemoveRange(notifications); // Remove all notifications
                await context.SaveChangesAsync(); // Save changes to the database
            }
        }



        public async Task UpdateAsync(Notification notification)
        {
            context.Notifications.Update(notification);
            await context.SaveChangesAsync();
        }
    }
}
