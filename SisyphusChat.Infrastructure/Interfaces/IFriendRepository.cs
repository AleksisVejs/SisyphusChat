using SisyphusChat.Infrastructure.Entities;


namespace SisyphusChat.Infrastructure.Interfaces;

public interface IFriendRepository : IRepository<Friend>
{
    public Task<ICollection<User>> GetAllFriendsAsync(string currentUserId);
}