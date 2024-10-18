using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class UserModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public byte[]? Picture { get; set; }

        public bool IsOnline { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public ICollection<ChatModel> Chats { get; set; }

        public ICollection<MessageModel> Messages { get; set; }

        public ICollection<FriendModel> Friends { get; set; }
    }
}