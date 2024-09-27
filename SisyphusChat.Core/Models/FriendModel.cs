using System.Dynamic;

namespace SisyphusChat.Core.Models
{
    public class FriendModel
    {
        public UserModel ReqSenderID { get; set; }
        
        public UserModel ReqReceiverID { get; set; }

        public bool IsAccepted { get; set; }
    }
}