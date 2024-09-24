using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class ChatModel
    {
        public string ID { get; set; }

        public string Name { get; set; }
        
        public ChatType Type { get; set; }

        public UserModel Owner { get; set; } 

        public bool IsReported { get; set; }
    }
}