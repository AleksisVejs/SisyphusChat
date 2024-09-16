using SisyphusChat.Infrastructure.Entities;


namespace SisyphusChat.Infrastructure.Interfaces;

public interface IChatRepository : IRepository<Chat>
{
    Task<Chat> GetPrivateChatAsync(string currentUserId, string recipientUserId);
}