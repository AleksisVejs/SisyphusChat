namespace SisyphusChat.Core.Models
{
    public class ChatUserModel
    {
        public string ChatID { get; set; }

        public ChatModel Chat { get; set; }

        public string UserID { get; set; }

        public UserModel User { get; set; }
    }
}