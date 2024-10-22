using SisyphusChat.Core.Models;


namespace SisyphusChat.Core.Interfaces
{
    public interface IFriendService : ICrud<FriendModel>
    {
        Task<ICollection<FriendModel>> GetAllFriendsAsync(string currentUserId);

        Task SendRequestAsync(string currentUserId, string recipientUserId);
    }
}
