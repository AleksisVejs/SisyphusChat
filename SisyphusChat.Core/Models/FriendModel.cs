using System.Dynamic;

namespace SisyphusChat.Core.Models
{
    public class FriendModel
    {
        public string ReqSenderId { get; set; }

        public UserModel ReqSender { get; set; }

        public string ReqReceiverId { get; set; }

        public UserModel ReqReceiver { get; set; }

        public bool IsAccepted { get; set; }
    }
}