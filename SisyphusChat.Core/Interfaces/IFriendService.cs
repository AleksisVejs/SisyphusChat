using SisyphusChat.Core.Models;


namespace SisyphusChat.Core.Interfaces
{
    public interface IFriendService : ICrud<FriendModel>
    {
        Task<ICollection<UserModel>> GetAllFriendsAsync(string currentUserId);

        Task SendRequestAsync(string currentUserId, string recipientUserId);
    }
}
