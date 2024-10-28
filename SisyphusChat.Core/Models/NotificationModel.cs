using System;

namespace SisyphusChat.Core.Models
{
    public class NotificationModel
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public UserModel User { get; set; }
        public string Type { get; set; }
        public string Message { get; set; }
        public string SenderUserId { get; set; } // Simple reference instead of full User entity
        public string SenderUsername { get; set; }
        public DateTime TimeCreated { get; set; }
        public bool IsRead { get; set; }
        public string RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
