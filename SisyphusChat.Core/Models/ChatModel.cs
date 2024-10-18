using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class ChatModel
    {
        public string Id { get; set; }

        public string? Name { get; set; }

        public ChatType Type { get; set; }

        public string OwnerId { get; set; }

        public bool IsReported { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public ICollection<ChatUser> ChatUsers { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
