using SisyphusChat.Infrastructure.Entities;
using System.Reflection.Metadata;

namespace SisyphusChat.Core.Models
{
    public class MessageModel
    {
        public ChatModel Chat { get; set; }
        
        public UserModel Sender { get; set; }
        
        public Blob Content { get; set; }

        public MessageStatus Status { Sent, Delivered, Read }
        
        public bool IsReported { get; set; }
    }
}