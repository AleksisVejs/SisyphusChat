using SisyphusChat.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using SisyphusChat.Infrastructure.Exceptions;

namespace SisyphusChat.Infrastructure.Repositories
{
    public class NotificationRepository(ApplicationDbContext context) : INotificationRepository
    {
        public async Task<ICollection<Notification>> GetAllAsync()
        {
            return await context.Notifications.ToListAsync();
        }
        public async Task AddAsync(Notification entity)
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.TimeCreated = DateTime.Now;
            await context.Notifications.AddAsync(entity);
            await context.SaveChangesAsync();
        }
        public async Task<Notification> GetByIdAsync(string id)
        {
            var notification = await context.Notifications.FirstOrDefaultAsync(g => g.Id == id);
            if (notification == null)
            {
                throw new EntityNotFoundException("Entity not found");
            }
            return notification;
        }

        public Task<List<Notification>> GetNotificationsByUserId(string userId)
        {
            return Task.FromResult(context.Notifications.Where(n => n.UserId == userId).ToList());
        }

        public Task<List<Notification>> GetUnreadNotificationsByUserId(string userId)
        {
            return Task.FromResult(context.Notifications.Where(n => n.UserId == userId && !n.IsRead).ToList());
        }

        public async Task DeleteByIdAsync(string id)
        {
            var notification = await context.Notifications.FindAsync(id);
            if (notification == null)
            {
                throw new EntityNotFoundException("Entity not found");
            }
            context.Notifications.Remove(notification);
            await context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Notification entity)
        {
            if (entity == null)
            {
                throw new EntityNotFoundException("Entity not found");
            }
            context.Notifications.Update(entity);
            await context.SaveChangesAsync();
        }

        public Task MarkAsRead(string notificationId)
        {
            var notification = context.Notifications.Find(notificationId);
            notification.IsRead = true;
            context.Notifications.Update(notification);
            return context.SaveChangesAsync();
        }

    }
}
