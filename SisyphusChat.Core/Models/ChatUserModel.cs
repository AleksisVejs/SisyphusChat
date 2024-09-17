namespace SisyphusChat.Core.Models
{
    public class ChatUserModel
    {
        public Guid ChatID { get; set; }

        public ChatModel Chat { get; set; }

        public Guid UserID { get; set; }

        public UserModel User { get; set; }
    }
}