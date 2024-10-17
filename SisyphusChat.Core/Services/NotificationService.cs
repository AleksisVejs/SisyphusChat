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
        public async Task AddNotificationAsync(string userId, string notificationType, string senderUsername)
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
                Message = message,
                NotificationType = notificationType,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            // Map to entity
            var notificationEntity = mapper.Map<Notification>(notificationModel);

            // Add the notification to the repository
            await unitOfWork.NotificationRepository.AddAsync(notificationEntity);
            await unitOfWork.SaveAsync();
        }



        public async Task ClearNotificationsAsync(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            await unitOfWork.NotificationRepository.DeleteByIdAsync(id);
            await unitOfWork.SaveAsync();
        }

        public async Task<IEnumerable<NotificationModel>> GetUserNotificationsAsync(string userId)
        {
            var notificationEntities = await unitOfWork.NotificationRepository.GetAllAsync();

            return mapper.Map<ICollection<Notification>, ICollection<NotificationModel>>(notificationEntities);
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
            await unitOfWork.NotificationRepository.DeleteByIdAsync(notificationId);

            // Save the changes to the database
            await unitOfWork.SaveAsync();
        }
    }
}
