using SisyphusChat.Infrastructure.Entities;


namespace SisyphusChat.Infrastructure.Interfaces;

public interface IFriendRepository : IRepository<Friend>
{
    public Task<ICollection<Friend>> GetAllFriendsAsync(string currentUserId);
}