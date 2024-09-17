using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class MessageModel
    {
        public ChatModel Chat { get; set; }
        
        public UserModel Sender { get; set; }
        
        public string Content { get; set; }

        public Attachment MessageAttachment { get; set; }

        public MessageStatus Status { Sent, Delivered, Read }
        
        public bool IsReported { get; set; }
    }
}