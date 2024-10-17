using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities
{
    public class Notification
    {
        [Key]
        public string Id { get; set; } // Primary key
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        //public string Type { get; set; }

        // You can add more fields if necessary, such as:
        public string RelatedEntityId { get; set; } // to ID the related entity (e.g., MessageId, UserId)
    }
}
