namespace SisyphusChat.Core.Models
{
    public class UserModel
    {
        public string ID { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public byte[]? Picture { get; set; }

        public bool IsOnline { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}