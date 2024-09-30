using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class MessageModel
    {
        public string Id { get; set; }

        public ChatModel ChatId { get; set; }
        
        public UserModel SenderId { get; set; }
        
        public string Content { get; set; }

        public MessageStatus Status { get; set; }
        
        public bool IsReported { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public Chat Chat { get; set; }
    }
}