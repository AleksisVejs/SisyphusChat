using SisyphusChat.Infrastructure.Entities;


namespace SisyphusChat.Infrastructure.Interfaces;

public interface IFriendRepository : IRepository<Friend>
{
    public Task<ICollection<User>> GetAllFriendsAsync(string currentUserId);

    public Task<ICollection<Friend>> GetAllSentRequestsAsync(string currentUserId);

    public Task<ICollection<Friend>> GetAllReceivedRequestsAsync(string currentUserId);
}