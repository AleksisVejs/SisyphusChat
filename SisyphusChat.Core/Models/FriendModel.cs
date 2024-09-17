using System.Dynamic;

namespace SisyphusChat.Core.Models
{
    public class FriendModel
    {
        public Guid ReqSenderID { get; set; }

        public UserModel ReqSender { get; set; }
        
        public Guid ReqReceiverID { get; set; }
        
        public UserModel ReqReceiver { get; set; }

        public bool IsAccepted { get; set; }
    }
}