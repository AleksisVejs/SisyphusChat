using SisyphusChat.Core.Models;


namespace SisyphusChat.Core.Interfaces
{
    public interface IFriendService : ICrud<FriendModel>
    {
        Task SendRequestAsync(string currentUserId, string recipientUserId);
    }
}
