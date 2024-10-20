using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Infrastructure.Interfaces;
public interface IUserRepository : IRepository<User>
{
    Task<User> GetByUsernameAsync(string username);

    Task<User> GetUserByChatIdAsync(string chatId);
}