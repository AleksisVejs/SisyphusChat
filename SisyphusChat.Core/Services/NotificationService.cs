using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using AutoMapper;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Services
{
    public class NotificationService(IUnitOfWork unitOfWork, IMapper mapper) : INotificationService
    {
        public async Task CreateAsync(NotificationModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var notificationEntity = mapper.Map<Notification>(model);

            await unitOfWork.NotificationRepository.AddAsync(notificationEntity);
            await unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(NotificationModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var notificationEntity = mapper.Map<Notification>(model);

            await unitOfWork.NotificationRepository.UpdateAsync(notificationEntity);
            await unitOfWork.SaveAsync();
        }

        public async Task<ICollection<NotificationModel>> GetAllAsync()
        {
            var notificationEntities = await unitOfWork.NotificationRepository.GetAllAsync();

            return mapper.Map<ICollection<Notification>, ICollection<NotificationModel>>(notificationEntities);
        }

        public async Task<NotificationModel> GetByIdAsync(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            var notificationEntity = await unitOfWork.NotificationRepository.GetByIdAsync(id);

            return mapper.Map<NotificationModel>(notificationEntity);
        }

        public async Task DeleteByIdAsync(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            await unitOfWork.NotificationRepository.DeleteByIdAsync(id);
            await unitOfWork.SaveAsync();
        }

        public async Task<List<NotificationModel>> GetNotificationsByUserId(string userId)
        {
            ArgumentException.ThrowIfNullOrEmpty(userId);

            var notificationEntities = await unitOfWork.NotificationRepository.GetNotificationsByUserId(userId);

            return mapper.Map<List<Notification>, List<NotificationModel>>(notificationEntities);
        }

        public async Task<List<NotificationModel>> GetUnreadNotificationsByUserId(string userId)
        {
            ArgumentException.ThrowIfNullOrEmpty(userId);

            var notificationEntities = await unitOfWork.NotificationRepository.GetUnreadNotificationsByUserId(userId);

            return mapper.Map<List<Notification>, List<NotificationModel>>(notificationEntities);
        }

        public async Task MarkAsRead(string notificationId)
        {
            ArgumentException.ThrowIfNullOrEmpty(notificationId);

            await unitOfWork.NotificationRepository.MarkAsRead(notificationId);
            await unitOfWork.SaveAsync();
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            ArgumentException.ThrowIfNullOrEmpty(userId);
            await unitOfWork.NotificationRepository.MarkAllAsReadAsync(userId);
            await unitOfWork.SaveAsync();
        }
    }
}
