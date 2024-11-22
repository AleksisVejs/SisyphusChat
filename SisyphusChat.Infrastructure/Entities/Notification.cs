using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities
{
    public class Notification
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public NotificationType Type { get; set; }

        [Required]
        [StringLength(1000)]
        public string Message { get; set; }

        [Required]
        [StringLength(256)]
        public string SenderUsername { get; set; }

        [Required]
        public DateTime TimeCreated { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        [StringLength(450)]
        public string RelatedEntityId { get; set; }
    }
}
