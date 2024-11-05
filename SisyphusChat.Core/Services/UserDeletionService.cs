using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Interfaces;
using SisyphusChat.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;


namespace SisyphusChat.Core.Services
{
    public class UserDeletionService : IUserDeletionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserDeletionService> _logger;

        public UserDeletionService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            ILogger<UserDeletionService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task DeleteUserAndRelatedDataAsync(string userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return;

                var deletedUser = await _userManager.FindByNameAsync("DELETED_USER");
                if (deletedUser == null)
                {
                    throw new InvalidOperationException("System deleted user not found");
                }

                await DeleteUserNotificationsAsync(userId);

                user.UserName = $"DELETED_USER_{userId}";
                user.NormalizedUserName = $"DELETED_USER_{userId}";
                user.Email = $"deleted_{userId}@deleted.com";
                user.NormalizedEmail = $"DELETED_{userId}@DELETED.COM";
                user.Picture = null;
                user.IsDeleted = true;

                await _userManager.UpdateAsync(user);
                await _unitOfWork.SaveAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting user data for userId: {UserId}", userId);
                throw;
            }
        }

        public async Task DeleteUserNotificationsAsync(string userId)
        {
            var notifications = await _unitOfWork.NotificationRepository.GetAllAsync();
            var userNotifications = notifications.Where(n => n.UserId == userId).ToList();

            foreach (var notification in userNotifications)
            {
                await _unitOfWork.NotificationRepository.DeleteByIdAsync(notification.Id);
            }
            await _unitOfWork.SaveAsync();
        }
    }
} 