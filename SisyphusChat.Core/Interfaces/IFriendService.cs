using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;


namespace SisyphusChat.Core.Interfaces
{
    public interface IFriendService : ICrud<FriendModel>
    {
        Task<ICollection<UserModel>> GetAllFriendsAsync(string currentUserId);

        Task<ICollection<FriendModel>> GetAllSentRequestsAsync(string currentUserId);

        Task<ICollection<FriendModel>> GetAllReceivedRequestsAsync(string currentUserId);

        Task SendRequestAsync(string currentUserId, string recipientUserId);
    }
}
