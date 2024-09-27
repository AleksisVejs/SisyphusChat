using System.Dynamic;

namespace SisyphusChat.Core.Models
{
    public class FriendModel
    {
        public UserModel ReqSenderId { get; set; }
        
        public UserModel ReqReceiverId { get; set; }

        public bool IsAccepted { get; set; }
    }
}