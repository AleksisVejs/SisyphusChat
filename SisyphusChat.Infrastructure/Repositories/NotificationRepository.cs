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

        public async Task<List<Notification>> GetNotificationsByUserId(string userId)
        {
            // Get all notifications for the user with related data
            var notifications = await context.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.TimeCreated)
                .ToListAsync();

            var filteredNotifications = new List<Notification>();
            
            foreach (var notification in notifications)
            {
                // Always include admin messages and group messages
                if (notification.Type == NotificationType.AdminMessage || 
                    notification.Type == NotificationType.FriendRequest ||
                    (notification.Type == NotificationType.Message && notification.Message.StartsWith("[")))
                {
                    filteredNotifications.Add(notification);
                    continue;
                }

                // For direct messages, check if users are friends
                var sender = await context.Users
                    .FirstOrDefaultAsync(u => u.UserName == notification.SenderUsername);
                    
                if (sender != null)
                {
                    var areFriends = await context.Friends
                        .AnyAsync(f => 
                            (f.ReqSenderId == userId && f.ReqReceiverId == sender.Id && f.IsAccepted) ||
                            (f.ReqReceiverId == userId && f.ReqSenderId == sender.Id && f.IsAccepted));

                    if (areFriends)
                    {
                        filteredNotifications.Add(notification);
                    }
                }
            }

            return filteredNotifications;
        }

        public async Task<List<Notification>> GetUnreadNotificationsByUserId(string userId)
        {
            // Get all unread notifications for the user
            var notifications = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            // Filter out direct message notifications from non-friends
            var filteredNotifications = new List<Notification>();
            
            foreach (var notification in notifications)
            {
                // Always include non-message notifications and group messages
                if (notification.Type != NotificationType.Message || notification.Message.StartsWith("["))
                {
                    filteredNotifications.Add(notification);
                    continue;
                }

                // For direct messages, check if users are friends
                var friendship1 = await context.Friends
                    .FirstOrDefaultAsync(f => 
                        (f.ReqSenderId == userId && f.ReqReceiverId == notification.SenderUsername) || 
                        (f.ReqReceiverId == userId && f.ReqSenderId == notification.SenderUsername));

                if (friendship1 != null && friendship1.IsAccepted)
                {
                    filteredNotifications.Add(notification);
                }
            }

            return filteredNotifications;
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

        public async Task MarkAllAsReadAsync(string userId)
        {
            var unreadNotifications = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await context.SaveChangesAsync();
        }

    }
}
