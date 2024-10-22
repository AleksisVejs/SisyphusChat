using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace SisyphusChat.Core.Services
{
    public class NotificationService(IUnitOfWork unitOfWork,IMapper mapper) : INotificationService
    {
        public async Task AddNotificationAsync(string userId, string notificationType, string senderUsername, string chatId)
        {
            // Validate inputs
            ArgumentNullException.ThrowIfNull(userId);
            ArgumentNullException.ThrowIfNull(notificationType);
            ArgumentNullException.ThrowIfNull(senderUsername);

            // Generate the message based on the notification type
            string message;
            if (NotificationTemplate.Templates.TryGetValue(notificationType, out var template))
            {
                message = template(senderUsername);
            }
            else
            {
                message = "You have a new notification."; // Default message
            }

            // Create the notification model
            var notificationModel = new NotificationModel
            {
                UserId = userId,
                SenderUsername = senderUsername, // Assign the sender's username here
                Message = message,
                NotificationType = notificationType,
                Timestamp = DateTime.UtcNow,
                ChatId = chatId,
                IsRead = false
                
            };

            // Map to entity
            var notificationEntity = mapper.Map<Notification>(notificationModel);

            try
            {
                await unitOfWork.NotificationRepository.AddAsync(notificationEntity);
                Console.WriteLine($"Notification added for userId: {userId}");
                await unitOfWork.SaveAsync();
                Console.WriteLine("Notifications saved successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while saving notification: {ex.Message}");
            }

        }




        public async Task ClearNotificationsAsync(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            await unitOfWork.NotificationRepository.DeleteByIdAsync(id);
            await unitOfWork.SaveAsync();
        }
        public async Task DeleteNotificationsByUsername(string userid,string username)
        {
            ArgumentNullException.ThrowIfNull(username);

            await unitOfWork.NotificationRepository.DeleteByUsernameAsync(userid,username);
            await unitOfWork.SaveAsync();
        }

        public async Task<List<NotificationModel>> GetUserNotificationsAsync(string userId)
        {
            var notificationEntities = await unitOfWork.NotificationRepository.GetUserNotificationsAsync(userId);

            return [.. mapper.Map<ICollection<Notification>, List<NotificationModel>>(notificationEntities)];
        }


        public async Task MarkAsReadAsync(string notificationId)
        {
            var notificationEntity = await unitOfWork.NotificationRepository.GetByIdAsync(notificationId);

            // Check if the notification exists
            if (notificationEntity == null)
            {
                throw new KeyNotFoundException($"Notification with ID {notificationId} not found.");
            }

            // Update the notification to mark it as read
            notificationEntity.IsRead = true;

            // Update the notification in the repository
            await unitOfWork.NotificationRepository.UpdateAsync(notificationEntity);

            // Save the changes to the database
            await unitOfWork.SaveAsync();
        }
    }
}
