using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class ChatModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ChatType Type { get; set; }

        public UserModel OwnerId { get; set; }

        public bool IsReported { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public ICollection<ChatUser> ChatUsers { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
