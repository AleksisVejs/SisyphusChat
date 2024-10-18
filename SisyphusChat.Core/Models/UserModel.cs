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

        public ICollection<Chat> Chats { get; set; }

        public ICollection<Message> Messages { get; set; }

        public ICollection<Friend> Friends { get; set; }
    }
}