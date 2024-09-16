using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Infrastructure.Interfaces;
public interface IUserRepository : IRepository<User>
{
    Task<User> GetUserByChatIdAsync(string chatId);
}