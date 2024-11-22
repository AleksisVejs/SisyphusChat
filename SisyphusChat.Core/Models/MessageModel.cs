using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class MessageModel
    {
        public string Id { get; set; }

        public string ChatId { get; set; }

        public ChatModel Chat { get; set; }
        
        public string SenderId { get; set; }

        public UserModel Sender { get; set; }
        
        public string Content { get; set; }

        public MessageStatus Status { get; set; }

        public bool IsEdited { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}