using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class MessageModel
    {
        public string ID { get; set; }

        public ChatModel ChatID { get; set; }
        
        public UserModel SenderID { get; set; }
        
        public string Content { get; set; }

        public MessageStatus Status { get; set; }
        
        public bool IsReported { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}