using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class ChatUserModel
    {
        public string UserId { get; set; }

        public ChatModel Chat { get; set; }

        public string ChatId { get; set; }

        public UserModel User { get; set; }
    }
}