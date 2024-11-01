using System.Dynamic;

namespace SisyphusChat.Core.Models
{
    public class FRequestModel
    {
        public IEnumerable<UserModel> Friends { get; set; }
        public IEnumerable<FriendModel> SentRequests { get; set; }
        public IEnumerable<FriendModel> ReceivedRequests { get; set; }
    }
}