using System;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class NotificationModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public UserModel User { get; set; }
        public NotificationType Type { get; set; }
        public string Message { get; set; }
        public string SenderUserId { get; set; }
        public string SenderUsername { get; set; }
        public DateTime TimeCreated { get; set; }
        public bool IsRead { get; set; }
        public string RelatedEntityId { get; set; }
    }
}
