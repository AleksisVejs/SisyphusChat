using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities
{
    [Table("Chats")]
    public class Chat : BaseEntity
    {
        [Required]
        [MaxLength(70)]
        public string Name { get; set; }

        [Required]
        public ChatType Type { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        [ForeignKey(nameof(OwnerId))]
        public User Owner { get; set; }

        [Required]
        public bool IsReported { get; set; }

        public ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}