using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class UserModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; }

        public bool IsBanned { get; set; }
        public DateTime? BanStart { get; set; }
        public DateTime? BanEnd { get; set; }

        public bool IsOnline { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastUpdated { get; set; }

        public DateTime? LastLogin { get; set; }
        public bool IsProfanityEnabled { get; set; }

        public byte[]? Picture { get; set; }

        public ICollection<ChatModel> Chats { get; set; }

        public ICollection<MessageModel> Messages { get; set; }

        public ICollection<FriendModel> Friends { get; set; }
    }
}